using System;

namespace DSH.DiscordBot.Contract.Dto
{
    public sealed class Source
    {
        public SourceType Type { get; set; }
        public Uri Url { get; set; }
    }
}