#if NETCOREAPP1_0
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
        private readonly Uri _baseUri;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly ILog _log;
        private readonly PactConfig _config;
        private IWebHost webHost;

       

        internal KestrelHttpHost(Uri baseUri, string providerName, PactConfig config, INancyBootstrapper bootstrapper) :
            this(baseUri, providerName, config, false)
        {
            _bootstrapper = bootstrapper;
        }

        internal KestrelHttpHost(Uri baseUri, string providerName, PactConfig config, bool bindOnAllAdapters)
        {
            var loggerName = LogProvider.CurrentLogProvider.AddLogger(config.LogDir, providerName.ToLowerSnakeCase(), "{0}_mock_service.log");
            config.LoggerName = loggerName;

            _baseUri = baseUri;
            _bootstrapper = new MockProviderNancyBootstrapper(config);
            _log = LogProvider.GetLogger(config.LoggerName);
            _config = config;

            /*
            _nancyConfiguration = new HostConfiguration
            {
                AllowChunkedEncoding = false
            };
            
            if (bindOnAllAdapters)
            {
                _nancyConfiguration.UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                };
                _nancyConfiguration.RewriteLocalhost = true;
            }
            else
            {
                _nancyConfiguration.RewriteLocalhost = false;
            }
            */
        }


        public void Start()
        {

            IWebHostBuilder host = new WebHostBuilder();
            host = host.UseContentRoot(Directory.GetCurrentDirectory());
            host = host.UseKestrel();
            
            host = host.UseStartup<Startup>();
            host = host.Configure(app =>
            {
                app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = _bootstrapper));
            });
            host = host.UseUrls(_baseUri.ToString());

            webHost = host.Build();
           
           webHost.Start();
        }

        public void Stop()
        {
            webHost.Dispose();
        }

        private readonly IConfiguration config;

        public KestrelHttpHost(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                              .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            var appConfig = new AppConfiguration();
            ConfigurationBinder.Bind(config, appConfig);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new DemoBootstrapper(appConfig)));
        }
    }
}
#endif