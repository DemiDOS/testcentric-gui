// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETFRAMEWORK
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using TestCentric.Common;
using TestCentric.Engine.Helpers;
using TestCentric.Engine.Internal;
using TestCentric.Engine.Services;

namespace TestCentric.Engine.Transports.Remoting
{
    /// <summary>
    /// Summary description for TestAgencyRemotingTransport.
    /// </summary>
    public class TestAgencyRemotingTransport : MarshalByRefObject, ITestAgencyTransport
    {
        private static readonly Logger log = InternalTrace.GetLogger(typeof(TestAgencyRemotingTransport));

        private string _uri;
        private int _port;

        private TcpChannel _channel;
        private bool _isMarshalled;

        private object _theLock = new object();

        public TestAgencyRemotingTransport(TestAgency agency)
        {
            Agency = agency;
            _uri = agency.Name;
            _port = 0; // Until Start() is called
        }

        public TestAgency Agency { get; }

        public string ConnectionPoint => ServerUrl;

        public string ServerUrl => $"tcp://127.0.0.1:{_port}/{_uri}";

        public bool IsRuntimeSupported(RuntimeFramework framework)
        {
            return framework.Runtime == Runtime.Net || framework.Runtime == Runtime.Mono;
        }

        public void Start()
        {
            if (_uri != null && _uri != string.Empty)
            {
                lock (_theLock)
                {
                    _channel = TcpChannelUtils.GetTcpChannel(_uri + "Channel", _port, 100);

                    RemotingServices.Marshal(this, _uri);
                    _isMarshalled = true;
                }

                if (_port == 0)
                {
                    ChannelDataStore store = this._channel.ChannelData as ChannelDataStore;
                    if (store != null)
                    {
                        string channelUri = store.ChannelUris[0];
                        _port = int.Parse(channelUri.Substring(channelUri.LastIndexOf(':') + 1));
                    }
                }
            }
        }

        [System.Runtime.Remoting.Messaging.OneWay]
        public void Stop()
        {
            lock( _theLock )
            {
                if ( this._isMarshalled )
                {
                    RemotingServices.Disconnect( this );
                    this._isMarshalled = false;
                }

                if ( this._channel != null )
                {
                    try
                    {
                        ChannelServices.UnregisterChannel(this._channel);
                        this._channel = null;
                    }
                    catch (RemotingException)
                    {
                        // Mono 4.4 appears to unregister the channel itself
                        // so don't do anything here.
                    }
                }

                Monitor.PulseAll( _theLock );
            }
        }

        public void Register(ITestAgent agent)
        {
            Agency.Register(agent);
        }

        public void WaitForStop()
        {
            lock( _theLock )
            {
                Monitor.Wait( _theLock );
            }
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

        /// <summary>
        /// Overridden to cause object to live indefinitely
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
#endif
