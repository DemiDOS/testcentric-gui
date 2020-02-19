// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TestCentric.Engine.Transports;

namespace TestCentric.Engine.Transports.NamedPipes
{
    /// <summary>
    /// RemoteTestAgentProxy wraps a RemoteTestAgent so that certain
    /// of its properties may be accessed without remoting.
    /// </summary>
    internal class RemoteTestAgentNamedPipeProxy : ITestAgent, ITestEngineRunner
    {
        private PipeStream _pipe;

        public RemoteTestAgentNamedPipeProxy(PipeStream pipe, Guid id)
        {
            _pipe = pipe;
            Id = id;
        }

        public Guid Id { get; private set; }

        public int CountTestCases(TestFilter filter)
        {
            new CountTestCasesCommand(filter).WriteTo(_pipe);
            return _pipe.ReadObject<int>();
        }

        public ITestEngineRunner CreateRunner(TestPackage package)
        {
            // TODO: Error if called more than once? Save package?
            new CreateRunnerCommand(package).WriteTo(_pipe);

            // Agent will continue to function as the runner
            return this;
        }

        public TestEngineResult Explore(TestFilter filter)
        {
            new ExploreCommand(filter).WriteTo(_pipe);
            return _pipe.ReadObject<TestEngineResult>();
        }

        public TestEngineResult Load()
        {
            new LoadCommand().WriteTo(_pipe);
            return _pipe.ReadObject<TestEngineResult>();
        }

        public TestEngineResult Reload()
        {
            new ReloadCommand().WriteTo(_pipe);
            return _pipe.ReadObject<TestEngineResult>();
        }

        public TestEngineResult Run(ITestEventListener listener, TestFilter filter)
        {
            new RunCommand(filter).WriteTo(_pipe);
            while (true)
            {
                var oResult = _pipe.ReadObject();
                var result = oResult as TestEngineResult;
                if (result != null)
                    return result;

                // If it's not a result, it's a progress report
                var report = oResult as string;
                if (report != null)
                    listener.OnTestEvent((string)oResult);
            }
        }

        public AsyncTestEngineResult RunAsync(ITestEventListener listener, TestFilter filter)
        {
            new RunAsyncCommand(filter).WriteTo(_pipe);
            return _pipe.ReadObject<AsyncTestEngineResult>();
        }

        public bool Start()
        {
            throw new InvalidOperationException("Cannot Start remote agent remotely");
        }

        public void Stop()
        {
            new StopCommand().WriteTo(_pipe);
        }

        public void StopRun(bool force)
        {
            new StopRunCommand(force).WriteTo(_pipe);
        }

        public void Unload()
        {
            new UnloadCommand().WriteTo(_pipe);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RemoteTestAgentNamedPipeProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif
