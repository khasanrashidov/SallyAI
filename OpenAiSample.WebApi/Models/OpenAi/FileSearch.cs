using Newtonsoft.Json;

namespace OpenAiSample.WebApi.Models.OpenAi;

public class FileSearch
{
    [JsonProperty("vector_store_ids")]
    public string[] VectorStoreIds { get; set; }
}
