using Newtonsoft.Json;

namespace DSH.DiscordBot.Infrastructure.Serialization
{
    public sealed class JsonNetSerilizer : ISerializer
    {
        public string Serialize<T>(T entity)
        {
            return JsonConvert.SerializeObject(entity, Formatting.Indented);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}