using System;

namespace DSH.DiscordBot.Contract.Dto
{
    public sealed class Build
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public Uri Url { get; set; }
    }
}