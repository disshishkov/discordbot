using Newtonsoft.Json;

namespace DSH.DiscordBot.Sources.Api.Entities
{
    public sealed class Build
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("desc")]
        public string Description { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}