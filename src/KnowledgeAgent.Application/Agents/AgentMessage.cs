using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeAgent.Application.Agents
{
    public sealed record AgentMessage(
        Guid ProjectId,
        string Message)
    {

    }
}
