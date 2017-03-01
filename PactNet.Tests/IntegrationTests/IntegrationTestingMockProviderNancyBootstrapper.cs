#if NETCOREAPP1_0
using Thinktecture.IO;
using PactNet.IO;
#else
using System.IO.Abstractions;
#endif
using NSubstitute;
using PactNet.Mocks.MockHttpService.Nancy;

namespace PactNet.Tests.IntegrationTests
{
    internal class IntegrationTestingMockProviderNancyBootstrapper : MockProviderNancyBootstrapper
    {
        public IntegrationTestingMockProviderNancyBootstrapper(PactConfig config)
            : base(config)
        {
        }

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register(typeof(IFileSystem), Substitute.For<IFileSystem>());
        }
    }
}