using System;
using NLog;

namespace DSH.DiscordBot.Infrastructure.Logging
{
    public sealed class NLogLogger : ILog
    {
        private readonly Logger _log;

        public NLogLogger()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public void Trace(String format, params Object[] args)
        {
            Log(LogLevel.Trace, format, args);
        }

        public void Debug(String format, params Object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public void Info(String format, params Object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void Warn(String format, params Object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        public void Warn(Exception ex, String format, params Object[] args)
        {
            Log(LogLevel.Warn, format, args, ex);
        }

        public void Error(String format, params Object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public void Error(Exception ex)
        {
            Log(LogLevel.Error, null, null, ex);
        }

        public void Error(Exception ex, String format, params Object[] args)
        {
            Log(LogLevel.Error, format, args, ex);
        }

        public void Fatal(Exception ex)
        {
            Log(LogLevel.Fatal, null, null, ex);
        }

        public void Fatal(String format, params Object[] args)
        {
            Log(LogLevel.Fatal, format, args);
        }

        public void Fatal(Exception ex, String format, params Object[] args)
        {
            Log(LogLevel.Fatal, format, args, ex);
        }

        private void Log(LogLevel level, String format, Object[] args)
        {
            _log.Log(typeof (NLogLogger), new LogEventInfo(level, _log.Name, null, format, args));
        }

        private void Log(LogLevel level, String format, Object[] args, Exception ex)
        {
            _log.Log(typeof (NLogLogger), new LogEventInfo(level, _log.Name, null, format, args, ex));
        }
    }
}