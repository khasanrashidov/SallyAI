using System.Text.Json.Serialization;

namespace OpenAiSample.WebApi.Models.Responses;

public class ExtractedMembersResponse
{
    [JsonPropertyName("members")]
    public string[] Members { get; set; }
}
