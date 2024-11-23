namespace TripifyBackend.INFRA.Entities.OpenAI;

public class OpenAIRoleMessage
{
    public string role { get; set; }
    public object content { get; set; }
}