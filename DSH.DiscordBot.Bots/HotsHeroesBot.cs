using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots
{
    public sealed class HotsHeroesBot : IHotsHeroesBot
    {
        public Hero GetHero(string name)
        {
            return new Hero()
            {
                Name = name,
                Builds = new [] {
                    new Build()
                {
                    Title = "Test Build",
                    Url = new Uri("https://duckduckgo.com")
                }}
            };
        }

        public IEnumerable<Hero> GetHeroes()
        {
            return new[]
            {
                new Hero()
                {
                    Name = "Абатур",
                    Aliases = new [] {"аба", "aba", "abatur"},
                    Builds = new [] {
                        new Build()
                        {
                            Title = "Минки",
                            Url = new Uri("http://blizzardheroes.ru/heroes/13-abatur/#bjBq")
                        }}
                },
                new Hero()
                {
                    Name = "Азмодан",
                    Aliases = new [] {"азмо", "azmo", "azmodan"},
                    Builds = new [] {
                        new Build()
                        {
                            Title = "Стаки сферы",
                            Url = new Uri("http://blizzardheroes.ru/heroes/31-azmodan/#kcc0")
                        }}
                }
            };
        }
    }
}