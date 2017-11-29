namespace DSH.DiscordBot.Clients
{
    public interface IDiscordClient
    {
        void Connect();
        void Disconnect();
    }
}