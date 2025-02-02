using System.Text.Json;

namespace OpenAiSample.WebApi.Services.JsonToExcel
{
    public class JsonService : IJsonService
    {
        public JsonElement ParseJson(string jsonString)
        {
            // Parse the JSON string and return the root element
            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            return jsonDocument.RootElement;
        }
    }
}
