using System.Text.Json;

namespace OpenAiSample.WebApi.Services.JsonToExcel
{
    public interface IExcelService
    {
        byte[] GenerateExcelFromJson(JsonElement jsonElement);

        byte[] GenerateRoadmapExcel(JsonElement jsonElement);
    }
}
