namespace DSH.DiscordBot.Infrastructure.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T entity);
        T Deserialize<T>(string json);
    }
}