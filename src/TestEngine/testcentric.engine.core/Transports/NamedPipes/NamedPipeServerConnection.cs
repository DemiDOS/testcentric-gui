// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NET40 || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeServerConnection : NamedPipeTransport
    {
        private NamedPipeServerStream _serverStream;

        public static NamedPipeServerConnection Create(string name)
        {
            return new NamedPipeServerConnection(CreateNamedPipeServerStream(name));
        }

        private NamedPipeServerConnection(NamedPipeServerStream serverStream)
            : base(serverStream)
        {
            _serverStream = serverStream;
        }

        protected static NamedPipeServerStream CreateNamedPipeServerStream(string name)
        {
            return new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        public void WaitForConnection()
        {
            _serverStream.WaitForConnection();
        }
    }
}
#endif
