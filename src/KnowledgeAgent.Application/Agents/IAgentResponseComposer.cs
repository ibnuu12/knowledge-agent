using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeAgent.Application.Agents
{
    public interface IAgentResponseComposer
    {
        AgentResult Compose(
        AgentMessage message,
        AgentContext context);
    }
}
