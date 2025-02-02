using System.Text.Json.Serialization;

namespace OpenAiSample.WebApi.Models.Responses;

public class CreateAssistantResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
