using Newtonsoft.Json;

namespace OpenAiSample.WebApi.Models.OpenAi;

public class Tool
{
    [JsonProperty("type")]
    public string Type { get; set; }
}
