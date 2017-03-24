using System.Collections.Generic;

namespace DSH.DiscordBot.Contract.Dto
{
    public sealed class Hero
    {
        public string Name { get; set; }
        public IEnumerable<Build> Builds { get; set; }
        public IEnumerable<string> Aliases { get; set; }
    }
}
