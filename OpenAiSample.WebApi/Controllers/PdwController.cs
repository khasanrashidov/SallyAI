using Microsoft.AspNetCore.Mvc;
using OpenAiSample.WebApi.Models.OpenAi;
using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;
using OpenAiSample.WebApi.Services;
using OpenAiSample.WebApi.Services.JsonToExcel;
using OpenApiSample.Data.Repositories;
using static OpenAiSample.WebApi.Constants;

namespace OpenAiSample.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdwController : ControllerBase
    {
        private readonly IPdwRepository _pdwRepository;
        private readonly IOpenAiService _openAiService;
        private readonly IExcelService _excelService;
        private readonly IJsonService _jsonService;

        public PdwController(IPdwRepository pdwRepository, IOpenAiService openAiService, IExcelService excelService, IJsonService jsonService)
        {
            _pdwRepository = pdwRepository;
            _openAiService = openAiService;
            _excelService = excelService;
            _jsonService = jsonService;
        }

        // download roadmap
        [HttpGet("download-roadmap/{pdwId}")]
        public async Task<IActionResult> DownloadRoadmap(int pdwId)
        {
            var pdw = await _pdwRepository.GetAsync(p => p.Id == pdwId);

            if (pdw == null)
            {
                return NotFound();
            }

            var createAssistantRequest = new CreateAssistantRequest
            {
                Model = "gpt-4o-mini",
                Name = AssistantType.PDWAssistant.ToString(),
                Tools = [new Tool { Type = "file_search" }],
                Instructions = AssistantInstructions.GetInstructions(AssistantType.RoadMapAssistant)
            };

            CreateAssistantResponse createdAssistant = await _openAiService.CreateAssistantsAsync(createAssistantRequest);

            var (generatedRoadmap, ideaThreadId) = await _openAiService.SendMessageAsync($"RETURN ONLY JSON: Provide a detailed and enhanced roadmap for this Project Discovery Workshop {pdw.JsonData}", createdAssistant.Id);

            var roadmapCleanJson = generatedRoadmap.Replace("```json", "").Replace("```", "").Trim();

            var jsonElement = _jsonService.ParseJson(roadmapCleanJson);
            var responseFile = _excelService.GenerateRoadmapExcel(jsonElement);

            return File(responseFile, "application/octet-stream", $"RoadMap.XLSX");
        }

    }
}
