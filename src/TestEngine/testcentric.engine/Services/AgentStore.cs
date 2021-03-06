// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric Engine contributors.
// Licensed under the MIT License. See LICENSE.txt in root directory.
// ***********************************************************************

#if !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestCentric.Engine.Services
{
    /// <summary>
    /// Defines the agent tracking operations that must be done atomically.
    /// </summary>
    internal sealed partial class AgentStore
    {
        private readonly Dictionary<Guid, AgentRecord> _agentsById = new Dictionary<Guid, AgentRecord>();

        public void AddAgent(Guid agentId, Process process)
        {
            lock (_agentsById)
            {
                if (_agentsById.ContainsKey(agentId))
                {
                    throw new ArgumentException($"An agent has already been started with the ID '{agentId}'.", nameof(agentId));
                }

                _agentsById.Add(agentId, AgentRecord.Starting(process));
            }
        }

        public void Register(ITestAgent agent)
        {
            lock (_agentsById)
            {
                if (!_agentsById.TryGetValue(agent.Id, out var record)
                    || record.Status != AgentStatus.Starting)
                {
                    throw new ArgumentException($"Agent {agent.Id} must have a status of {AgentStatus.Starting} in order to register, but the status was {record.Status}.", nameof(agent));
                }

                _agentsById[agent.Id] = record.Ready(agent);
            }
        }

        public bool IsReady(Guid agentId, out ITestAgent agent)
        {
            lock (_agentsById)
            {
                if (_agentsById.TryGetValue(agentId, out var record)
                    && record.Status == AgentStatus.Ready)
                {
                    agent = record.Agent;
                    return true;
                }

                agent = null;
                return false;
            }
        }

        public bool IsAgentProcessActive(Guid agentId, out Process process)
        {
            lock (_agentsById)
            {
                if (_agentsById.TryGetValue(agentId, out var record)
                    && record.Status != AgentStatus.Terminated)
                {
                    process = record.Process;
                    return process != null;
                }

                process = null;
                return false;
            }
        }

        public void MarkTerminated(Guid agentId)
        {
            lock (_agentsById)
            {
                if (!_agentsById.TryGetValue(agentId, out var record))
                {
                    throw new ArgumentException($"An entry for agent {agentId} must exist in order to mark it as terminated.", nameof(agentId));
                }

                _agentsById[agentId] = record.Terminated();
            }
        }
    }
}
#endif
