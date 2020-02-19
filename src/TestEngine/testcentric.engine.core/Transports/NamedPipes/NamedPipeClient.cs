// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeClient
    {
        private const int CONNECT_DELAY = 5000;

        private string _basePipeName;
        private Guid _guid;

        public NamedPipeClient(string name, Guid guid)
        {
            _basePipeName = name;
            _guid = guid;
        }

        public NamedPipeClientConnection GetDataConnection()
        {
            var initialConnection = NamedPipeClientConnection.Create(_basePipeName);
            initialConnection.Connect(CONNECT_DELAY);

            // Identify myself by sending my Id as an unprefixed byte array
            initialConnection.WriteBytes(_guid.ToByteArray());

            // Get the name of the actual data connection to use
            var dataPipeName = initialConnection.ReadObject<string>();
            initialConnection.Dispose();

            // Create and connect to the data pipe
            var dataConnection = NamedPipeClientConnection.Create(dataPipeName);
            dataConnection.Connect(CONNECT_DELAY);

            return dataConnection;
        }
    }
}
#endif
