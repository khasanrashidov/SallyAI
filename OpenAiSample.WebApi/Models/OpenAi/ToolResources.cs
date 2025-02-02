using Newtonsoft.Json;

namespace OpenAiSample.WebApi.Models.OpenAi;

public class ToolResources
{
    [JsonProperty("file_search")]
    public FileSearch FileSearch { get; set; }
}
