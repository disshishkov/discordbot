using System;
using System.Collections.Generic;
using System.Linq;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Storage;

namespace DSH.DiscordBot.Bots
{
    public sealed class HotsHeroesBot : IHotsHeroesBot
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IStorage> _storage;
        private readonly Lazy<IScreenshoter> _screenshoter;

        public HotsHeroesBot(
            Lazy<ILog> log,
            Lazy<ISerializer> serializer,
            Lazy<IStorage> storage,
            Lazy<IScreenshoter> screenshoter)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _screenshoter = screenshoter ?? throw new ArgumentNullException(nameof(screenshoter));
        }

        public Hero GetHero(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _log.Value.Info($"Getting hero '{name}'");

            var id = GetId(name);

            return _storage.Value.Fetch<Hero>(_ => _.Id == id).FirstOrDefault();
        }

        public Hero GetHeroByAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));
            
            _log.Value.Info($"Getting hero by alias'{alias}'");

            alias = alias.ToUpperInvariant();
            
            var hero = _storage.Value.Fetch<Hero>(_ => _.Id == alias 
                   || (_.Aliases ?? new string[0]).Contains(alias))
                .FirstOrDefault();

            if (hero?.Builds != null)
            {
                foreach (var build in hero.Builds)
                {
                    build.Screen = _storage.Value.GetData(GetBuildDataId(hero.Id, build.Url));
                }
            }

            return hero;
        }

        public IEnumerable<Hero> GetHeroes()
        {
            _log.Value.Info("Getting heroes list");

            return _storage.Value.All<Hero>();
        }

        public void DeleteHero(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            _log.Value.Info($"Deleting hero '{name}'");
            
            var id = GetId(name);
            
            _storage.Value.Delete<Hero>(_ => _.Id == id);
            _storage.Value.DeleteData(GetBuildsDataPreffix(id));
        }

        public void SaveAlias(string heroName, string alias)
        {
            if (string.IsNullOrWhiteSpace(heroName))
                throw new ArgumentNullException(nameof(heroName));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            _log.Value.Info($"Adding alias '{alias}' to hero '{heroName}'");

            alias = alias.ToUpperInvariant();
            var hero = GetHero(heroName);

            if (hero == null)
            {
                _log.Value.Debug($"Hero '{heroName}' is absent need to create a new");

                InsertHero(new Hero()
                {
                    Name = heroName,
                    Aliases = new[] {alias}
                });
            }
            else
            {
                _log.Value.Debug($"Hero '{heroName}' was found need update the alias");

                if (hero.Aliases == null)
                {
                    hero.Aliases = new[] {alias};
                }

                if (hero.Aliases.All(a => a != alias))
                {
                    hero.Aliases = hero.Aliases.Concat(new[] {alias});
                }

                _storage.Value.Update(hero);
            }
        }

        public string SaveBuild(string heroName, Build build)
        {
            if (string.IsNullOrWhiteSpace(heroName))
                throw new ArgumentNullException(nameof(heroName));
            if (build == null)
                throw new ArgumentNullException(nameof(build));
            if (string.IsNullOrWhiteSpace(build.Title))
                throw new ArgumentNullException(nameof(build.Title));
            if (build.Url == null)
                throw new ArgumentNullException(nameof(build.Url));

            _log.Value.Info($"Adding build '{build.Title}' to hero '{heroName}'");

            var isNeedScreenshot = false;
            var hero = GetHeroByAlias(heroName);

            if (hero == null)
            {
                _log.Value.Debug($"Hero '{heroName}' is absent need to create a new");

                hero = new Hero()
                {
                    Name = heroName,
                    Builds = new[] {build}
                };

                InsertHero(hero);
                isNeedScreenshot = true;
                hero.Id = GetId(hero.Name);
            }
            else
            {
                _log.Value.Debug($"Hero '{heroName}' was found need update the build");

                if (hero.Builds == null)
                {
                    hero.Builds = new[] {build};
                    isNeedScreenshot = true;
                }

                if (hero.Builds.All(_ => _.Url.AbsoluteUri != build.Url.AbsoluteUri))
                {
                    hero.Builds = hero.Builds.Concat(new[] {build});
                    isNeedScreenshot = true;
                }

                _storage.Value.Update(hero);
            }

            if (isNeedScreenshot)
            {
                var data = _screenshoter.Value.Take(build.Url);
                if (data != null)
                {
                    _storage.Value.InsertData(
                        GetBuildDataId(hero.Id, build.Url), data);
                }
            }

            return hero.Name;
        }

        public void SaveScreen(string heroName, string url)
        {
            if (string.IsNullOrWhiteSpace(heroName))
                throw new ArgumentNullException(nameof(heroName));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            
            _log.Value.Info($"Saving screenshot '{url}' for hero '{heroName}'");

            var hero = GetHero(heroName);
            if (hero != null)
            {
                var buildUrl = new Uri(url);
                var dataId = GetBuildDataId(hero.Id, buildUrl);
                    
                var data = _storage.Value.GetData(dataId);
                if (data == null)
                {
                    data = _screenshoter.Value.Take(buildUrl);
                    if (data != null)
                    {
                        _storage.Value.InsertData(dataId, data);
                    }
                }
            }
        }

        public void SaveHeroes(IEnumerable<Hero> heroes)
        {
            if (heroes == null)
                throw new ArgumentNullException(nameof(heroes));

            _log.Value.Info("Save heroes with builds");

            foreach (var hero in heroes)
            {
                if (hero.Builds == null)
                    continue;
                
                var addedBuilds = new Dictionary<string, IList<string>>();
                var screensToDelete = new List<string>();
                var heroName = hero.Name;
                var heroId = heroName;

                foreach (var build in hero.Builds)
                {
                    if (string.IsNullOrWhiteSpace(build.Source))
                        continue;
                    if (build.Url == null)
                        continue;

                    heroName = SaveBuild(hero.Name, build);
                    heroId = GetId(heroName);

                    if (!addedBuilds.ContainsKey(build.Source))
                    {
                        addedBuilds.Add(build.Source, new List<string>() {build.Url.AbsoluteUri});
                    }
                    else
                    {
                        addedBuilds[build.Source].Add(build.Url.AbsoluteUri);
                    }
                }

                _log.Value.Debug(
                    $"Added builds for hero '{heroName}':{Environment.NewLine}{_serializer.Value.Serialize(addedBuilds)}");

                foreach (var addedBuild in addedBuilds)
                {
                    _log.Value.Info($"Clear old builds for hero '{heroName}' from source '{addedBuild.Key}'");

                    var heroInStorage = GetHero(heroName);
                    var builds = heroInStorage.Builds
                        .Where(_ => _.Source != addedBuild.Key || addedBuild.Value.Contains(_.Url.AbsoluteUri))
                        .ToArray();

                    foreach (var build in heroInStorage.Builds)
                    {
                        if (!builds.Select(_ => _.Url.AbsoluteUri).Contains(build.Url.AbsoluteUri))
                        {
                            screensToDelete.Add(GetBuildDataId(heroId, build.Url));
                        }
                    }
                    
                    heroInStorage.Builds = builds;
                    heroInStorage.ImageUrl = hero.ImageUrl;
                    _storage.Value.Update(heroInStorage);
                }
                
                _log.Value.Info($"Remove {screensToDelete.Count} old screenshots for hero {heroName}");
                foreach (var dataId in screensToDelete)
                {
                    _storage.Value.DeleteData(dataId);
                }
                
                _log.Value.Info($"Get screenshots for hero {heroName}");
                foreach (var buildUrl in addedBuilds.Values.SelectMany(_ => _))
                {
                    var url = new Uri(buildUrl);
                    var dataId = GetBuildDataId(heroId, url);
                    
                    var data = _storage.Value.GetData(dataId);
                    if (data == null)
                    {
                        data = _screenshoter.Value.Take(url);
                        if (data != null)
                        {
                            _storage.Value.InsertData(dataId, data);
                        }
                    }
                }
            }
        }

        public void DeleteAllHeroes()
        {
            _log.Value.Info("Delete all heroes");
            
            _storage.Value.Drop<Hero>();
        }

        public Build ParseBuild(string buildStr)
        {
            var build = new Build();
            build.Title = "Default";

            if (string.IsNullOrWhiteSpace(buildStr))
                return build;

            var parts = buildStr.Split('|');

            // Just url is presented
            if (parts.Length == 1)
            {
                build.Url = new Uri(parts[0].ToLowerInvariant());
            }

            // Title and url are presented
            if (parts.Length == 2)
            {
                build.Title = parts[0];
                build.Url = new Uri(parts[1].ToLowerInvariant());
            }

            // Title, url and source are presented
            if (parts.Length == 3)
            {
                build.Title = parts[0];
                build.Url = new Uri(parts[1].ToLowerInvariant());
                build.Source = parts[2];
            }

            return build;
        }

        private static string GetId(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return name.ToUpperInvariant();
        }

        private static string GetBuildsDataPreffix(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return $"$/{ClearDataId(id)}/builds/";
        }

        private static string GetBuildDataId(string heroId, Uri buildUrl)
        {
            if (string.IsNullOrWhiteSpace(heroId))
                throw new ArgumentNullException(nameof(heroId));
            if (buildUrl == null)
                throw new ArgumentNullException(nameof(buildUrl));

            return $"{GetBuildsDataPreffix(heroId)}{ClearDataId(buildUrl.AbsoluteUri)}";
        }

        private static string ClearDataId(string str)
        {
            return str
                .Replace("/", "-")
                .Replace(" ", "-")
                .Replace(":", "-")
                .Replace("'", "-")
                .Replace("#", "-");
        }

        private void InsertHero(Hero hero)
        {
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            hero.Id = GetId(hero.Name);

            _storage.Value.Insert(hero);
        }
    }
}