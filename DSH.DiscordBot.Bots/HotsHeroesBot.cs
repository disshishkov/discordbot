using System;
using System.Collections.Generic;
using System.Linq;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Storage;

namespace DSH.DiscordBot.Bots
{
    public sealed class HotsHeroesBot : IHotsHeroesBot
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IStorage> _storage;

        public HotsHeroesBot(
            Lazy<ILog> log,
            Lazy<ISerializer> serializer,
            Lazy<IStorage> storage)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            _log = log;
            _serializer = serializer;
            _storage = storage;
        }

        public Hero GetHero(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _log.Value.Info("Getting hero '{0}'", name);

            var id = GetId(name);

            return _storage.Value.Fetch<Hero>(_ => _.Id == id).FirstOrDefault();
        }

        public IEnumerable<Hero> GetHeroes()
        {
            _log.Value.Info("Getting heroes list");

            return _storage.Value.All<Hero>();
        }

        public void SaveAlias(string heroName, string alias)
        {
            if (string.IsNullOrWhiteSpace(heroName))
                throw new ArgumentNullException(nameof(heroName));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            _log.Value.Info("Adding alias '{0}' to hero '{1}'", alias, heroName);

            var hero = GetHero(heroName);

            if (hero == null)
            {
                _log.Value.Debug("Hero '{0}' is absent need to create a new", heroName);

                InsertHero(new Hero()
                {
                    Name = heroName,
                    Aliases = new[] {alias}
                });
            }
            else
            {
                _log.Value.Debug("Hero '{0}' was found need update the alias", heroName);

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

        public void SaveBuild(string heroName, Build build)
        {
            if (string.IsNullOrWhiteSpace(heroName))
                throw new ArgumentNullException(nameof(heroName));
            if (build == null)
                throw new ArgumentNullException(nameof(build));
            if (string.IsNullOrWhiteSpace(build.Title))
                throw new ArgumentNullException(nameof(build.Title));
            if (build.Url == null)
                throw new ArgumentNullException(nameof(build.Url));

            _log.Value.Info("Adding build '{0}' to hero '{1}'", build.Title, heroName);

            var hero = GetHero(heroName);

            if (hero == null)
            {
                _log.Value.Debug("Hero '{0}' is absent need to create a new", heroName);

                InsertHero(new Hero()
                {
                    Name = heroName,
                    Builds = new[] {build}
                });
            }
            else
            {
                _log.Value.Debug("Hero '{0}' was found need update the build", heroName);

                if (hero.Builds == null)
                {
                    hero.Builds = new[] {build};
                }

                if (hero.Builds.All(_ => _.Url.AbsoluteUri != build.Url.AbsoluteUri))
                {
                    hero.Builds = hero.Builds.Concat(new[] {build});
                }

                _storage.Value.Update(hero);
            }
        }

        public void SaveHeroes(IEnumerable<Hero> heroes)
        {
            if (heroes == null)
                throw new ArgumentNullException(nameof(heroes));

            _log.Value.Info("Save heroes with builds");

            foreach (var hero in heroes)
            {
                var addedBuilds = new Dictionary<string, IList<string>>();

                foreach (var build in hero.Builds)
                {
                    SaveBuild(hero.Name, build);

                    if (!addedBuilds.ContainsKey(build.Source))
                    {
                        addedBuilds.Add(build.Source, new List<string>() {build.Url.AbsoluteUri});
                    }
                    else
                    {
                        addedBuilds[build.Source].Add(build.Url.AbsoluteUri);
                    }
                }

                _log.Value.Debug("Added builds for hero '{1}':{0}{2}",
                    Environment.NewLine,
                    hero.Name,
                    _serializer.Value.Serialize(addedBuilds));

                foreach (var addedBuild in addedBuilds)
                {
                    _log.Value.Info("Clear old builds for hero '{0}' from source '{1}'", hero.Name, addedBuild.Key);

                    var heroInStorage = GetHero(hero.Name);
                    var builds = heroInStorage.Builds
                        .Where(_ => _.Source != addedBuild.Key || addedBuild.Value.Contains(_.Url.AbsoluteUri))
                        .ToArray();

                    heroInStorage.Builds = builds;
                    _storage.Value.Update(heroInStorage);
                }
            }
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

        private void InsertHero(Hero hero)
        {
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            hero.Id = GetId(hero.Name);

            _storage.Value.Insert(hero);
        }
    }
}