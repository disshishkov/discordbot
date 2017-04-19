using System.Threading.Tasks;

namespace DSH.DiscordBot.Infrastructure.Web
{
    public interface IClient
    {
        Task<string> GetString(string url);
    }
}