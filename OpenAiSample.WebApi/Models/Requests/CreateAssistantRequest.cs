using Newtonsoft.Json;
using OpenAiSample.WebApi.Models.OpenAi;

namespace OpenAiSample.WebApi.Models.Requests;

public class CreateAssistantRequest
{
    [JsonProperty("model")]
    public string Model { get; set; } = "gpt-4o-mini";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("instructions")]
    public string Instructions { get; set; }

    [JsonProperty("tools")]
    public Tool[] Tools { get; set; }

    [JsonProperty("tool_resources")]
    public ToolResources ToolResources { get; set; }
}
