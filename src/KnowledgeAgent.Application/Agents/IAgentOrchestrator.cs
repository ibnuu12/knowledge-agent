using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeAgent.Application.Agents
{
    public interface IAgentOrchestrator
    {
        Task<AgentResult> ProcessAsync(
        AgentMessage message,
        CancellationToken cancellationToken);
    }
}
