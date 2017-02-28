using System;
using Nancy.Bootstrapper;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Mocks.MockHttpService.Kestrel;

namespace PactNet.Mocks.MockHttpService
{
    internal class PactHttpHost : IHttpHost
    {
        private readonly IHttpHost _httpHost;
        internal PactHttpHost(Uri baseUri, string providerName, PactConfig config, INancyBootstrapper bootstrapper)
        {
#if NETCOREAPP1_0
            _httpHost = new KestrelHttpHost(baseUri, providerName, config, bootstrapper);
#else
            _httpHost = new NancyHttpHost(baseUri, providerName, config, bootstrapper);
#endif
        }

        internal PactHttpHost(Uri baseUri, string providerName, PactConfig config, bool bindOnAllAdapters)
        {
#if NETCOREAPP1_0
            _httpHost = new KestrelHttpHost(baseUri, providerName, config, bindOnAllAdapters);
#else
            _httpHost = new NancyHttpHost(baseUri, providerName, config, bindOnAllAdapters);
#endif
        }

        public void Start()
        {
            _httpHost.Start();
        }

        public void Stop()
        {
            _httpHost.Stop();
        }
    }
}
