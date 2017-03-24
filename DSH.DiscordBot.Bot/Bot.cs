using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bot
{
    public sealed class Bot : IBot
    {
        public Hero GetHero(string name)
        {
            return new Hero()
            {
                Name = name,
                Builds = new [] {new Build()
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
                new Hero() {Name = "Test1", Aliases = new [] {"test1", "t1", "т1"}},
                new Hero() {Name = "Test2", Aliases = new [] {"test2", "t2", "т2"}}
            };
        }
    }
}