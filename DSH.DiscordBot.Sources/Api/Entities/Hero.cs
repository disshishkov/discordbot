using Newtonsoft.Json;

namespace DSH.DiscordBot.Sources.Api.Entities
{
    public sealed class Hero
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("builds")]
        public Build[] Builds { get; set; }
    }
}