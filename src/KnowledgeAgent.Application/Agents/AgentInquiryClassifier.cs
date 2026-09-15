namespace KnowledgeAgent.Application.Agents;

internal sealed class AgentInquiryClassifier
{
    public AgentInquiryType Classify(string message)
    {
        var normalized = message.Trim().ToLowerInvariant();

        if (normalized.Contains("requirement"))
        {
            return AgentInquiryType.Requirements;
        }

        if (normalized.Contains("decision"))
        {
            return AgentInquiryType.Decisions;
        }

        if (normalized.Contains("assumption"))
        {
            return AgentInquiryType.Assumptions;
        }

        if (normalized.Contains("current state") ||
            normalized.Contains("project state"))
        {
            return AgentInquiryType.CurrentState;
        }

        return AgentInquiryType.Unknown;
    }
}