#if NET40 || NETCOREAPP2_1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TestCentric.Engine.Transports.NamedPipes
{
    //public class NamedPipeServerTests
    //{
    //    private NamedPipeServer _server;
    //    private NamedPipeClient _client;
    //    private NamedPipeClientConnection _connection;

    //    [SetUp]
    //    public void CreateServer()
    //    {
    //        _server = new NamedPipeServer("Testing");
    //        _client = new NamedPipeClient("Testing", Guid.NewGuid());
    //        _server.Start();
    //        _connection = _client.GetDataConnection();
    //    }

    //    [TearDown]
    //    public void StopServer()
    //    {
    //        _connection.Dispose();
    //        //_server.Stop();
    //    }

    //    [Test]
    //    public void ConnectionTest()
    //    {
    //        Assert.NotNull(_connection);
    //    }
    //}
}
#endif
