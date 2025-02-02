using System.Text.Json.Serialization;

namespace OpenAiSample.WebApi.Models.Responses;

public class GeneratedIdeasResponse
{
    [JsonPropertyName("ideas")]
    public IdeaDto[] Ideas { get; set; }
}
