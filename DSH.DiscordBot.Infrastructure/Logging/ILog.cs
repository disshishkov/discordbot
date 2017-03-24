using System;

namespace DSH.DiscordBot.Infrastructure.Logging
{
    public interface ILog
    {
        void Trace(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Info(string format, params object[] args);
        void Warn(string format, params object[] args);
        void Warn(Exception ex, string format, params object[] args);
        void Error(string format, params object[] args);
        void Error(Exception ex);
        void Error(Exception ex, string format, params object[] args);
        void Fatal(Exception ex);
        void Fatal(string format, params object[] args);
        void Fatal(Exception ex, string format, params object[] args);
    }
}