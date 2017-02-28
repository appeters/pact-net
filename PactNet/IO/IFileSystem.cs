#if NETSTANDARD1_5
using System;
using System.Collections.Generic;
using Thinktecture.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PactNet.IO
{
    public interface IFileSystem
    {
         IFile File { get; }
        IDirectory Directory { get; }
    }
}
#endif
