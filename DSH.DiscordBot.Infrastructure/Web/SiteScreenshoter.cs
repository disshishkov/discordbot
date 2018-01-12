using System;
using System.IO;
using System.Text;
using DSH.DiscordBot.Infrastructure.Logging;
using NReco.PhantomJS;

namespace DSH.DiscordBot.Infrastructure.Web
{
    public sealed class SiteScreenshoter : IScreenshoter
    {
        private readonly Lazy<ILog> _log;
        
        public SiteScreenshoter(Lazy<ILog> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public byte[] Take(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            _log.Value.Trace($"Take screenshot of {url.AbsoluteUri}");
            
            try
            {
                var phantomJs = new PhantomJS();
                phantomJs.ToolPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "PhantomJs");

                using (var ms = new MemoryStream())
                {
                    phantomJs.Run(
                        Path.Combine(phantomJs.ToolPath, "screenshot.js"), 
                        new[] {url.AbsoluteUri}, 
                        null, ms);

                    var base64Bytes = ms.ToArray();
                
                    if (base64Bytes.Length <= 0)
                        return null;
                
                    return Convert.FromBase64String(
                        Encoding.UTF8.GetString(base64Bytes, 0, base64Bytes.Length));
                }
            }
            catch (Exception e)
            {
                _log.Value.Error(e);
                return null;
            }
        }
    }
}