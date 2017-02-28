using System;
using System.IO;
#if NETSTANDARD1_5
using Thinktecture.IO;
using Thinktecture;
using PactNet.IO;
#else
using System.IO.Abstractions;
#endif
using System.Linq;
using System.Text;



namespace PactNet.Infrastructure.Logging
{
    internal class LocalRollingLogFileMessageHandler : ILocalLogMessageHandler
    {
        public string LogPath { get; set; }

        private readonly object _sync = new object();
        private readonly StreamWriter _writer;

        internal LocalRollingLogFileMessageHandler(IFileSystem fileSystem, string logFilePath)
        {
            LogPath = logFilePath;
            TryCreateDirectory(logFilePath);

#if NETSTANDARD1_5
            var file = fileSystem.File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read).ToImplementation();
#else
            var file = fileSystem.File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
#endif

            _writer = new StreamWriter(file, Encoding.UTF8);
        }

        internal LocalRollingLogFileMessageHandler(string logFilePath)
            : this(new FileSystem(), logFilePath)
        {
        }

        public void Handle(LocalLogMessage logMessage)
        {
            var messageFormat = logMessage.MessagePredicate != null ?
                logMessage.MessagePredicate() :
                String.Empty;

            string message;
            if (logMessage.Exception != null)
            {
                message = String.Format("{0}. Exception: {1} - {2}", messageFormat, logMessage.Exception, logMessage.Exception.StackTrace);
            }
            else if (logMessage.FormatParameters != null && logMessage.FormatParameters.Any())
            {
                message = String.Format(messageFormat, logMessage.FormatParameters);
            }
            else
            {
                message = messageFormat;
            }
            
            lock (_sync)
            {
                _writer.WriteLine("{0} [{1}] {2}", logMessage.DateTimeFormatted, logMessage.Level, message);
                _writer.Flush();
            }
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
            }
        }

        private static void TryCreateDirectory(string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create log directory.", ex);
            }
        }
    }
}