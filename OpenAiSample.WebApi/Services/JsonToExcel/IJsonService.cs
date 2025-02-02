using System.Text.Json;

namespace OpenAiSample.WebApi.Services.JsonToExcel
{
    public interface IJsonService
    {
        public JsonElement ParseJson(string jsonString);
    }
}
