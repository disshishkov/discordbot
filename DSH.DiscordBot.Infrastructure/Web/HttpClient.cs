using System.Threading.Tasks;

namespace DSH.DiscordBot.Infrastructure.Web
{
    public sealed class HttpClient : IClient
    {
        private readonly System.Net.Http.HttpClient _client;

        public HttpClient()
        {
            _client = new System.Net.Http.HttpClient();
        }

        public async Task<string> GetString(string url)
        {
            return await _client.GetStringAsync(url);
        }
    }
}