using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSH.DiscordBot.Clients
{
    public interface IDiscordClient
    {
        void Connect();
        void Disconnect();
        void AddCommand(string name, IEnumerable<string> aliases, Func<Task<string>> func);
        void AddAdminCommand(string name, Func<string, string, Task<string>> func);
    }
}