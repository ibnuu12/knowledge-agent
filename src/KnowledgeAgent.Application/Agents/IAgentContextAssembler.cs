namespace KnowledgeAgent.Application.Agents;

public interface IAgentContextAssembler
{
    Task<AgentContext> AssembleAsync(
        AgentMessage message,
        CancellationToken cancellationToken);
}