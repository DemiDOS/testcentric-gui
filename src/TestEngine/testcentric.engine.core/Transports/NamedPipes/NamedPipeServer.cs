// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeServer
    {
        private const int GUID_BUFFER_SIZE = 16;

        private string _basePipeName;

        private List<NamedPipeServerConnection> _connections;
        private bool _serverIsRunning;

        public NamedPipeServer(string name)
        {
            _basePipeName = name;
        }

        public delegate void ConnectionEventHandler(NamedPipeServerConnection connection, Guid id);

        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        public event ConnectionEventHandler ClientConnected;

        public void Start()
        {
            _connections = new List<NamedPipeServerConnection>();
            _serverIsRunning = true;

            new TaskFactory().StartNew(() =>
            {
                while (_serverIsRunning)
                {
                    WaitForClientConnection();
                }
            });
        }

        private void WaitForClientConnection()
        {
            var serverConnection = NamedPipeServerConnection.Create(_basePipeName);
            serverConnection.WaitForConnection();

            // Upon connection, remote agent must immediately send its Id as identification.
            // Guid is sent as a raw byte array, without any preceding length specified.
            Guid id = new Guid(serverConnection.ReadBytes(GUID_BUFFER_SIZE));

            // Now send the actual data transer pipe name and close the data stream.
            // This is done so that each client uses a different pipe name, thereby
            // avoiding running out of pipe instances.
            var dataPipeName = GetNextDataPipeName();
            serverConnection.WriteObject(dataPipeName);
            serverConnection.Dispose();

            var dataConnection = NamedPipeServerConnection.Create(dataPipeName);
            dataConnection.WaitForConnection();

            _connections.Add(dataConnection);
            ClientConnected?.Invoke(dataConnection, id);
        }

        public void Stop()
        {
            _serverIsRunning = false;

            foreach (var connection in _connections)
            {
                connection.Dispose();
            }

            // Server loop will only exit between connections, so
            // we make a dummy connection to force it to terminate.
            var dummyClient = new NamedPipeClientStream(_basePipeName);
            dummyClient.Connect(1000);
            dummyClient.Close();
        }

        private int _nextPipeId;

        private string GetNextDataPipeName()
        {
            return $"{_basePipeName}_{++_nextPipeId}";
        }
    }
}
#endif
