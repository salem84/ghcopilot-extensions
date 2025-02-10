namespace DabAgent.Models;

public class AgentPayload
{
    public bool Stream { get; set; }
    public string Model { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = [];
}
