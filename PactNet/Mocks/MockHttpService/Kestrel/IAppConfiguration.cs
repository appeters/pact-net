using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PactNet.Mocks.MockHttpService.Kestrel
{
    public interface IAppConfiguration
    {
        Logging Logging { get; }
        Smtp Smtp { get; }
    }
}