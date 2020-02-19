// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETCOREAPP2_1 || NET40
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class NamedPipeTransportTests
    {
        private const int CONNECT_DELAY = 100;
        private const int WRITE_DELAY = 100;

        private static readonly TestPackage _originalPackage = new TestPackage(new string[] { "mock-assembly.dll", "notest-assembly.dll" });

        private NamedPipeServerConnection _server;
        NamedPipeClientConnection _client;

        [SetUp]
        public void CreateTransports()
        {
            var name = "Testing_" + Guid.NewGuid().ToString("N");
            _server = NamedPipeServerConnection.Create(name);
            _client = NamedPipeClientConnection.Create(name);
        }

        [TearDown]
        public void CloseTransports()
        {
            _client.Dispose();
            _server.Dispose();
        }

        [Test]
        public void ServerWritesToClient()
        {
            // Run Server
            RunTask(() =>
            {
                TestContext.Progress.WriteLine("Server waiting");
                _server.WaitForConnection();

                // Delay allows client to issue the read before server writes
                Thread.Sleep(WRITE_DELAY);

                TestContext.Progress.WriteLine("Server writing");
                _server.WriteObject(_originalPackage);
            });

            TestPackage newPackage = null;

            // Run Client
            TestContext.Progress.WriteLine("Client connecting");
            _client.Connect(CONNECT_DELAY);

            TestContext.Progress.WriteLine("Client reading");
            newPackage = _client.ReadObject<TestPackage>();

            ComparePackages(newPackage, _originalPackage);
        }

        [Test]
        public void ClientWritesToServer()
        {
            TestPackage newPackage = null;

            // Run Server
            RunTask(() =>
            {
                TestContext.Progress.WriteLine("Server waiting");
                _server.WaitForConnection();

                TestContext.Progress.WriteLine("Server reading");
                newPackage = _server.ReadObject<TestPackage>();
            });

            // Run Client
            TestContext.Progress.WriteLine("Client connecting");
            _client.Connect(CONNECT_DELAY);

            // Delay allows server to issue the read before client writes
            Thread.Sleep(WRITE_DELAY);

            TestContext.Progress.WriteLine("Client writing");
            _client.WriteObject(_originalPackage);

            Thread.Sleep(WRITE_DELAY);

            ComparePackages(newPackage, _originalPackage);
        }

        [Test]
        public void SendCommands()
        {
            var commands = new string[] { "One", "Two", "Three", "Four", "Five", "Six" };

            // Run Server
            RunTask(() =>
            {
                TestContext.Progress.WriteLine("Server waiting");
                _server.WaitForConnection();
                TestContext.Progress.WriteLine("Server connected");

                Thread.Sleep(WRITE_DELAY);

                foreach (var command in commands)
                {
                    TestContext.Progress.WriteLine($"Server writing command {command}");
                    _server.WriteObject(new TestCommand(command));
                }
            });

            var received = new List<string>();

            // Run Client
            TestContext.Progress.WriteLine("Client connecting");
            _client.Connect(CONNECT_DELAY);

            for (int i = 0; i < 6; i++)
            {
                var name = _client.ReadObject<NamedPipeCommand>().Name;
                TestContext.Progress.WriteLine($"Client read comand {name}");
                received.Add(name);
            }

            Assert.That(received, Is.EqualTo(commands));
        }

        private void RunTask(Action action)
        {
            // Can't use Task.Run on .NET 4.0
            Task.Factory.StartNew(action);
        }

        private void ComparePackages(TestPackage newPackage, TestPackage oldPackage)
        {
            Assert.That(newPackage.Name, Is.EqualTo(oldPackage.Name));
            Assert.That(newPackage.FullName, Is.EqualTo(oldPackage.FullName));
            Assert.That(newPackage.Settings.Count, Is.EqualTo(oldPackage.Settings.Count));
            Assert.That(newPackage.SubPackages.Count, Is.EqualTo(oldPackage.SubPackages.Count));

            foreach (var key in oldPackage.Settings.Keys)
            {
                Assert.That(newPackage.Settings.ContainsKey(key));
                Assert.That(newPackage.Settings[key], Is.EqualTo(oldPackage.Settings[key]));
            }

            for (int i = 0; i < oldPackage.SubPackages.Count; i++)
                ComparePackages(newPackage.SubPackages[i], oldPackage.SubPackages[i]);
        }

        [Serializable]
        private class TestCommand : NamedPipeCommand
        {
            public TestCommand(string command) : base(command) { }
        }
    }
}
#endif
