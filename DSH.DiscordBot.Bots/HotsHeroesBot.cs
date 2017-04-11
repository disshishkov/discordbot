using System;
using System.Collections.Generic;
using System.Linq;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Storage;

namespace DSH.DiscordBot.Bots
{
    public sealed class HotsHeroesBot : IHotsHeroesBot
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IStorage> _storage;

        public HotsHeroesBot(Lazy<ILog> log, Lazy<IStorage> storage)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            _log = log;
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