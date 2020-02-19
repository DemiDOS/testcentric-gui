// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if NETSTANDARD2_0 || NET40
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using TestCentric.Engine.Agents;
using TestCentric.Engine.Internal;

namespace TestCentric.Engine.Transports.NamedPipes
{
    public class TestAgentNamedPipeTransport : ITestAgentTransport, ITestEventListener
    {
        private static readonly Logger log = InternalTrace.GetLogger(typeof(TestAgentNamedPipeTransport));

        private readonly string _agencyUrl;

        ITestEngineRunner _runner;
        NamedPipeClientConnection _connection;

        public TestAgentNamedPipeTransport(RemoteTestAgent agent, string agencyUrl)
        {
            Agent = agent;
            _agencyUrl = agencyUrl;
        }
        
        public TestAgent Agent { get; }

        public bool Start()
        {
            var client = new NamedPipeClient(_agencyUrl, Agent.Id);
            _connection = client.GetDataConnection();

            Thread commandLoop = new Thread(CommandLoop);
            commandLoop.Start();

            return true;
        }

        public void Stop()
        {
            Agent.StopSignal.Set();
        }

        public ITestEngineRunner CreateRunner(TestPackage package)
        {
            return Agent.CreateRunner(package);
        }

        private void CommandLoop()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                var command = _connection.ReadCommand();

                switch (command.Name)
                {
                    case "CreateRunner":
                        var package = ((CreateRunnerCommand)command).Package;
                        _runner = CreateRunner(package);
                        break;
                    case "Load":
                        _connection.WriteResult(_runner.Load());
                        break;
                    case "Reload":
                        _connection.WriteResult(_runner.Reload());
                        break;
                    case "Unload":
                        _runner.Unload();
                        break;
                    case "Explore":
                        var filter = ((ExploreCommand)command).Filter;
                        _connection.WriteResult(_runner.Explore(filter));
                        break;
                    case "CountTestCases":
                        filter = ((CountTestCasesCommand)command).TestFilter;
                        _connection.WriteResult(_runner.CountTestCases(filter));
                        break;
                    case "Run":
                        filter = ((RunCommand)command).TestFilter;
                        _connection.WriteResult(_runner.Run(this, filter));
                        break;
                    case "RunAsync":
                        filter = ((RunAsyncCommand)command).TestFilter;
                        _runner.RunAsync(this, filter);
                        break;
                    case "Stop":
                        keepRunning = false;
                        break;
                }
            }

            Stop();
        }

        public void OnTestEvent(string report)
        {
            _connection.WriteObject(report);
        }
    }

    [Serializable]
    public class CreateRunnerCommand : NamedPipeCommand
    {
        public readonly TestPackage Package;

        public CreateRunnerCommand(TestPackage package)
            : base("CreateRunner")
        {
            Package = package;
        }
    }

    [Serializable]
    public class StopCommand : NamedPipeCommand
    {
        public StopCommand() : base("Stop") { }
    }

    [Serializable]
    public class LoadCommand : NamedPipeCommand
    {
        public LoadCommand() : base("Load") { }
    }

    [Serializable]
    public class ReloadCommand : NamedPipeCommand
    {
        public ReloadCommand() : base("Reload") { }
    }

    [Serializable]
    public class UnloadCommand : NamedPipeCommand
    {
        public UnloadCommand() : base("Unload") { }
    }

    [Serializable]
    public class ExploreCommand : NamedPipeCommand
    {
        public readonly TestFilter Filter;

        public ExploreCommand(TestFilter filter)
            : base("Explore")
        {
            Filter = filter;
        }
    }

    [Serializable]
    public class CountTestCasesCommand : NamedPipeCommand
    {
        public TestFilter TestFilter;

        public CountTestCasesCommand(TestFilter filter)
            : base("CountTestCases")
        {
            TestFilter = filter;
        }
    }

    [Serializable]
    public class RunCommand : NamedPipeCommand
    {
        public readonly TestFilter TestFilter;

        public RunCommand(TestFilter filter)
            : base("Run")
        {
            TestFilter = filter;
        }
    }

    [Serializable]
    public class RunAsyncCommand : NamedPipeCommand
    {
        public TestFilter TestFilter;

        public RunAsyncCommand(TestFilter filter)
            : base("RunAsync")
        {
            TestFilter = filter;
        }
    }

    [Serializable]
    public class StopRunCommand : NamedPipeCommand
    {
        public bool ForcedStop;

        public StopRunCommand(bool force)
            : base("StopRun")
        {
            ForcedStop = force;
        }
    }
}
#endif
