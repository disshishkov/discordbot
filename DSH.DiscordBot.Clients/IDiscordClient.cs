using System;
using System.Collections.Generic;

namespace DSH.DiscordBot.Clients
{
    public interface IDiscordClient
    {
        void Connect();
        void Disconnect();
        void AddCommand(string name, IEnumerable<string> aliases, string answer);
    }
}