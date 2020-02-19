// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NET40 || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeClientConnection : NamedPipeTransport
    {
        private NamedPipeClientStream _clientStream;

        public static NamedPipeClientConnection Create(string name)
        {
            return new NamedPipeClientConnection(CreateNamedPipeClientStream(name));
        }

        private NamedPipeClientConnection(NamedPipeClientStream clientStream)
            : base(clientStream)
        {
            _clientStream = clientStream;
        }

        protected static NamedPipeClientStream CreateNamedPipeClientStream(string name)
        {
            var options = RunningOnWindows
                ? PipeOptions.Asynchronous | PipeOptions.WriteThrough
                : PipeOptions.Asynchronous;
            return new NamedPipeClientStream(".", name, PipeDirection.InOut, options);
        }

        public void Connect(int timeout)
        {
            _clientStream.Connect(timeout);
            if (RunningOnWindows)
                _clientStream.ReadMode = PipeTransmissionMode.Message;
        }
    }
}
#endif
