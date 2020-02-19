// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using TestCentric.Engine.Agents;
using TestCentric.Engine.Internal;
using TestCentric.Engine.Services;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class TestAgencyNamedPipeTransport : ITestAgencyTransport
    {
        private static readonly Logger log = InternalTrace.GetLogger(typeof(TestAgencyNamedPipeTransport));
        private const int GUID_BUFFER_SIZE = 16;

        NamedPipeServer _server;

        public TestAgencyNamedPipeTransport(TestAgency agency)
        {
            Agency = agency;
            var uniqueId = Guid.NewGuid().ToString("N");
            ConnectionPoint = $"{agency.Name}_{uniqueId}";
        }

        public TestAgency Agency { get; }

        public string ConnectionPoint { get; }

        public bool IsRuntimeSupported(RuntimeFramework framework)
        {
            return framework.Runtime == Runtime.NetCore;
        }

        public void Start()
        {
            _server = new NamedPipeServer(ConnectionPoint);
            _server.ClientConnected += (connection, id) => Agency.Register(new RemoteTestAgentNamedPipeProxy(connection.PipeStream, id));

            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        public void Register(ITestAgent agent)
        {
            Agency.Register(agent);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Stop();

                _disposed = true;
            }
        }
    }
}
#endif
