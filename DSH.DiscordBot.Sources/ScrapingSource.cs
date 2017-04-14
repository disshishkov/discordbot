using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Sources
{
    public sealed class ScrapingSource : ISource
    {
        public IEnumerable<Hero> GetHeroes()
        {
            return new List<Hero>()
            {
                new Hero()
                {
                    Name = "Аларак",
                    Builds = new List<Build>()
                    {
                        new Build()
                        {
                            Title = "Основной",
                            Source = "HappyZerg",
                            Url = new Uri("http://www.heroesfire.com/hots/talent-calculator/alarak#kFai")
                        },
                        new Build()
                        {
                            Title = "Основной",
                            Source = "Test",
                            Url = new Uri("http://test2.com")
                        }
                    }
                },

                new Hero()
                {
                    Name = "Ануб'арак",
                    Builds = new List<Build>()
                    {
                        new Build()
                        {
                            Title = "Основной",
                            Source = "HappyZerg",
                            Url = new Uri("http://blizzardheroes.ru/heroes/30-anubarak/#ljra")
                        }
                    }
                }
            };
        }
    }
}