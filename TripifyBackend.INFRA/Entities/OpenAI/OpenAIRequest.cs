namespace TripifyBackend.INFRA.Entities.OpenAI;

public class OpenAIRequest
{
    public List<OpenAIPlacesList> Places { get; set; }
    public OpenAIUserPreferences UserPreferences { get; set; }
}