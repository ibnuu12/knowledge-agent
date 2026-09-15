using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeAgent.Application.Agents
{
    public sealed record AgentResult(string Response,
    string CurrentStateSummary,
    IReadOnlyList<string> Evidence,
    IReadOnlyList<string> OpenQuestions,
    IReadOnlyList<string> ProposedActions,
    bool RequiresAuthorization)
    {

    }
}
