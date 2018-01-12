using System;

namespace DSH.DiscordBot.Infrastructure.Web
{
    public interface IScreenshoter
    {
        byte[] Take(Uri url);
    }
}