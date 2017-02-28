#if NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace PactNet.IO
{
    public class FileSystem :IFileSystem
    {
        private readonly IFile _file = new FileAdapter();
        private readonly IDirectory _directory = new DirectoryAdapter();

        IDirectory IFileSystem.Directory
        {
            get
            {
                return _directory;
            }
        }

        IFile IFileSystem.File
        {
            get
            {
               return _file;
            }
        }

        /*
        public FileSystem(IFile file)
        {
            _file = file;
        }
        */
        public string GetContent(string path)
        {
            using (IFileStream stream = _file.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (IStreamReader reader = new StreamReaderAdapter(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
#endif