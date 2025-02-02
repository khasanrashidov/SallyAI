using Microsoft.AspNetCore.Mvc;
using OpenAiSample.WebApi.Models.OpenAi;
using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;
using OpenAiSample.WebApi.Services;
using OpenAiSample.WebApi.Services.JsonToExcel;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Repositories;
using static OpenAiSample.WebApi.Constants;

namespace OpenAiSample.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdeaController : ControllerBase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IOpenAiService _openAiService;
    private readonly IExcelService _excelService;
    private readonly IJsonService _jsonService;
    private readonly IPdwRepository _pdwRepository;
    private readonly IProjectRepository _projectRepository;

    public IdeaController(IIdeaRepository ideaRepository, IOpenAiService openAiService, IExcelService excelService,
        IJsonService jsonService, IPdwRepository pdwRepository, IProjectRepository projectRepository)
    {
        _ideaRepository = ideaRepository;
        _openAiService = openAiService;
        _excelService = excelService;
        _jsonService = jsonService;
        _pdwRepository = pdwRepository;
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ideas = await _ideaRepository.GetAllAsync();
        return Ok(ideas);
    }

    [HttpGet("ByProject/{projectId}")]
    public async Task<IActionResult> GetByProjectId(int projectId)
    {
        var ideas = await _ideaRepository.GetAllAsync(i => i.ProjectId == projectId);

        return Ok(ideas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var idea = await _ideaRepository.GetAsync(i => i.Id == id);
        if (idea == null)
        {
            return NotFound();
        }
        return Ok(idea);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Idea idea)
    {
        if (idea == null)
        {
            return BadRequest();
        }

        await _ideaRepository.AddAsync(idea);
        return CreatedAtAction(nameof(Get), new { id = idea.Id }, idea);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Idea idea)
    {
        if (idea == null || idea.Id != id)
        {
            return BadRequest();
        }

        var existingIdea = await _ideaRepository.GetAsync(i => i.Id == id);
        if (existingIdea == null)
        {
            return NotFound();
        }

        await _ideaRepository.UpdateAsync(idea);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var idea = await _ideaRepository.GetAsync(i => i.Id == id);
        if (idea == null)
        {
            return NotFound();
        }

        await _ideaRepository.DeleteAsync(idea);
        return NoContent();
    }

    // mark true the TechLeadApproved property of an idea
    [HttpPut("tech-lead-approve/{id}")]
    public async Task<IActionResult> TechLeadApprove(int id)
    {
        var idea = await _ideaRepository.GetAsync(i => i.Id == id);
        if (idea == null)
        {
            return NotFound();
        }

        idea.TechLeadApproved = true;

        await _ideaRepository.UpdateAsync(idea);

        return Ok(idea);
    }

    // mark true the ClientApproved property of an idea
    [HttpPut("client-approve/{id}")]
    public async Task<IActionResult> ClientApprove(int id)
    {
        var idea = await _ideaRepository.GetAsync(i => i.Id == id);
        if (idea == null)
        {
            return NotFound();
        }

        idea.ClientApproved = true;

        await _ideaRepository.UpdateAsync(idea);

        var createAssistantRequest = new CreateAssistantRequest
        {
            Model = "gpt-4o-mini",
            Name = AssistantType.PDWAssistant.ToString(),
            Tools = [new Tool { Type = "file_search" }],
            Instructions = AssistantInstructions.GetInstructions(AssistantType.PDWAssistant)
        };

        CreateAssistantResponse createdAssistant = await _openAiService.CreateAssistantsAsync(createAssistantRequest);

        var (generatedPwd, ideaThreadId) = await _openAiService.SendMessageAsync($"Provide PDW for this idea: {idea.Name} with description: {idea.Description}", createdAssistant.Id);


        var pdwCleanJson = generatedPwd.Replace("```json", "").Replace("```", "").Trim();

        var pdw = new Pdw
        {
            JsonData = pdwCleanJson,
        };
        var createdPwd = await _pdwRepository.AddAsync(pdw);

        var obtainedProject = await _projectRepository.GetAsync(p => p.Id == idea.ProjectId);

        obtainedProject.PdwId = createdPwd.Id;

        await _projectRepository.UpdateAsync(obtainedProject);

        var jsonElement = _jsonService.ParseJson(pdwCleanJson);
        var responseFile = _excelService.GenerateExcelFromJson(jsonElement);

        return File(responseFile, "application/octet-stream", $"PDW_Report_{idea.Name}.XLSX");
    }
}
