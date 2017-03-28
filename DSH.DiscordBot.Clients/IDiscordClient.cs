using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSH.DiscordBot.Clients
{
    public interface IDiscordClient
    {
        void Connect();
        void Disconnect();
        void AddCommand(string name, IEnumerable<string> aliases, string answer);
        void AddAdminCommand(string name, string answer, Func<Task> func);
    }
}