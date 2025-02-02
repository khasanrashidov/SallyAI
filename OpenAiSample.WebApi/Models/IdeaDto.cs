using System.Text.Json.Serialization;

namespace OpenAiSample.WebApi.Models;

public class IdeaDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("followUpQuestions")]
    public string[] FollowUpQuestions { get; set; }
}
