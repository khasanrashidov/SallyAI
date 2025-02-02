using Newtonsoft.Json;

namespace OpenAiSample.WebApi.Models.Requests;

public class CreateVectorStoreFileRequest
{
    [JsonProperty("file_id")]
    public string FileId { get; set; }
}
