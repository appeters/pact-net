#if NETCOREAPP1_0
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nancy.Bootstrapper;
using Nancy.Owin;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Nancy;

namespace PactNet.Mocks.MockHttpService.Kestrel
{
    public class KestrelHttpHost : IHttpHost
    {
        private readonly string _baseUriString;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly ILog _log;
        private readonly PactConfig _config;
        private IWebHost _webHost;

        internal KestrelHttpHost(Uri baseUri, string providerName, PactConfig config, INancyBootstrapper bootstrapper) :
            this(baseUri, providerName, config, false)
        {
            _bootstrapper = bootstrapper;
        }

        internal KestrelHttpHost(Uri baseUri, string providerName, PactConfig config, bool bindOnAllAdapters)
        {
            var loggerName = LogProvider.CurrentLogProvider.AddLogger(config.LogDir, providerName.ToLowerSnakeCase(), "{0}_mock_service.log");
            config.LoggerName = loggerName;

            _baseUriString = baseUri.ToString();
            _bootstrapper = new MockProviderNancyBootstrapper(config);
            _log = LogProvider.GetLogger(config.LoggerName);
            _config = config;

            if (bindOnAllAdapters)
            {
                var builder = new UriBuilder(baseUri);
                _baseUriString = _baseUriString.Replace(builder.Host, "*");
            }
        }

        public void Start()
        {
            IWebHostBuilder host = new WebHostBuilder();
            host = host.UseContentRoot(Directory.GetCurrentDirectory());
            host = host.UseKestrel();
            host = host.Configure(app =>
            {
                app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = _bootstrapper));
            });
            host = host.UseUrls(_baseUriString);

            _webHost = host.Build();
            _webHost.Start();
        }

        public void Stop()
        {
            _webHost.Dispose();
        }
    }
}
#endif