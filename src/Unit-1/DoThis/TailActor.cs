using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        private IActorRef _reporterActor;
        private string _filePath;
        private FileObserver _observer;
        private Stream _fileStream;
        private StreamReader _fileStreamReader;

        #region Messages

        public class FileWrite
        {
            public readonly string FilePath;

            public FileWrite(string filePath)
            {
                FilePath = filePath;
            }
        }

        public class FileError
        {
            public readonly string FileName;
            public readonly string Reason;

            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }
        }

        public class InitialRead
        {
            public readonly string FileName;
            public readonly string Text;

            public InitialRead(string fileName, string text)
            {
                Text = text;
                FileName = fileName;
            }
        }

        #endregion

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;
            
        }

        protected override void PreStart()
        {
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            _fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream);

            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, text));
        }

        protected override void PostStop()
        {
            _observer.Dispose();
            _observer=null;
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reporterActor.Tell(text);
                }
            }
            else if (message is FileError)
            {
                var fileError = message as FileError;
                _reporterActor.Tell(string.Format("Tail error: {0}", fileError.Reason));
            }

            else if (message is InitialRead)
            {
                var initialRead = message as InitialRead;
                _reporterActor.Tell(initialRead.Text);
            }

        }

    }
}