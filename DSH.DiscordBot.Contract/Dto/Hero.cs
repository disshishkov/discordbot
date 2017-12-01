using System;
using System.Collections.Generic;

namespace DSH.DiscordBot.Contract.Dto
{
    public sealed class Hero
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri ImageUrl { get; set; }
        public IEnumerable<Build> Builds { get; set; }
        public IEnumerable<string> Aliases { get; set; }
    }
}
