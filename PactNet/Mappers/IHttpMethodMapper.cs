using System.Net.Http;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace PactNet.Mappers
{
    public interface IHttpMethodMapper
    {
        HttpMethod Convert(HttpVerb from);
    }
}