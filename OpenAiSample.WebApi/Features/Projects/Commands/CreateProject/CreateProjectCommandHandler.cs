using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OpenAiSample.WebApi.Models;
using OpenAiSample.WebApi.Models.OpenAi;
using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;
using OpenAiSample.WebApi.Services;
using OpenAiSample.WebApi.Services.Api;
using OpenAiSample.WebApi.Services.Identity;
using OpenApiSample.Data;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Repositories;
using System.Text.Json;
using static OpenAiSample.WebApi.Constants;

namespace OpenAiSample.WebApi.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IOpenAiApi _openAiApi;
    private readonly IOpenAiService _openAiService;
    private readonly OpenAiConfig _openAiConfig;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly AppDbContext _context;
    private readonly IIdeaRepository _ideaRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUserProjectRepository _userProjectRepository;

    public CreateProjectCommandHandler(IProjectsRepository projectsRepository,
        IOpenAiApi openAiApi,
        IOpenAiService openAiService,
        UserManager<User> userManager,
        ICurrentUserService currentUserService,
        AppDbContext context,
        IIdeaRepository ideaRepository,
        IMemberRepository memberRepository,
        IUserProjectRepository userProjectRepository,
        IOptionsMonitor<OpenAiConfig> optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(projectsRepository);
        ArgumentNullException.ThrowIfNull(openAiApi);

        _projectsRepository = projectsRepository;
        _openAiApi = openAiApi;
        _openAiService = openAiService;
        _openAiConfig = optionsMonitor.CurrentValue;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _context = context;
        _ideaRepository = ideaRepository;
        _memberRepository = memberRepository;
        _userProjectRepository = userProjectRepository;
    }

    public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstAsync(
                u => u.Email == _currentUserService.UserEmail!
                );

        CreateVectorStoreResponse createdVectorStore = await _openAiApi.CreateVectorStoreAsync(new CreateVectorStoreRequest() { Name = $"{request.Name}-vs" });

        UploadFileResponse uploadTranscript;
        using (var formDataContent = new MultipartFormDataContent())
        {
            formDataContent.Add(new StreamContent(request.Transcript.OpenReadStream()), "file", request.Transcript.FileName);
            formDataContent.Add(new StringContent(OpenAiConstants.Purpose), "purpose");

            uploadTranscript = await _openAiApi.UploadFileAsync(formDataContent);
        }

        UploadFileResponse uploadSummary;
        using (var formDataContent = new MultipartFormDataContent())
        {
            formDataContent.Add(new StreamContent(request.Summary.OpenReadStream()), "file", request.Summary.FileName);
            formDataContent.Add(new StringContent(OpenAiConstants.Purpose), "purpose");

            uploadSummary = await _openAiApi.UploadFileAsync(formDataContent);
        }

        await _openAiService.CreateVectorStoreFileAsync(createdVectorStore.Id, new CreateVectorStoreFileRequest() { FileId = uploadTranscript.Id });
        await _openAiService.CreateVectorStoreFileAsync(createdVectorStore.Id, new CreateVectorStoreFileRequest() { FileId = uploadSummary.Id });

        var tools = new Tool[] { new() { Type = "file_search" } };

        var createAssistantRequest = new CreateAssistantRequest
        {
            Model = "gpt-4o-mini",
            Name = AssistantType.IdeaGenerator.ToString(),
            Tools = [new Tool { Type = "file_search" }],
            ToolResources = new ToolResources
            {
                FileSearch = new FileSearch
                {
                    VectorStoreIds = [createdVectorStore.Id]
                }
            },
            Instructions = AssistantInstructions.GetInstructions(AssistantType.IdeaGenerator)
        };

        CreateAssistantResponse createdAssistant = await _openAiService.CreateAssistantsAsync(createAssistantRequest);

        // TODO: Look at documentation to check how to wait till the response is generated
        var (generatedIdeas, ideaThreadId) = await _openAiService.SendMessageAsync($"Geterate ideas based on description: {request.Description} and months needed: {request.MonthsNeeded}", createdAssistant.Id);

        var ideas = GetIdeas(generatedIdeas);

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            MonthsNeeded = request.MonthsNeeded,
            ClientName = request.ClientName,
            VectorStoreId = createdVectorStore.Id,
            AssistantId = createdAssistant.Id,
            State = 1,
            Users = [user]
        };

        Project? createdProject;
        try
        {
            createdProject = await _projectsRepository.CreateProjectAsync(project);
        }
        catch (Exception ex)
        {
            return null;
        }
        
        var techLead = await _userManager.Users.FirstAsync(u => u.Id == request.TechLeadId);

        await _userProjectRepository.AddAsync(new UserProject{ UserId = techLead.Id, ProjectId = createdProject.Id });

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var idea in ideas)
        {
            var newIdea = new Idea
            {
                ProjectId = createdProject.Id,
                Name = idea.Name,
                Description = idea.Description,
                RequirementQuestions = string.Join(", ", idea.FollowUpQuestions),
            };

            await _ideaRepository.AddAsync(newIdea);
        }

        var (extractedMembers, memberThreadId) = await _openAiService.SendMessageAsync(AssistantInstructions.GetInstructions(AssistantType.MeetingMemberExtractor), createdAssistant.Id);

        var members = GetMembersFullNamesAsync(extractedMembers);

        foreach (var fullName in members)
        {
            var member = new Member
            {
                ProjectId = createdProject.Id,
                FullName = fullName
            };

            await _memberRepository.AddAsync(member);
        }

        return createdProject;
    }

    private static IdeaDto[] GetIdeas(string generatedIdeas)
    {
        try
        {
            var cleanJson = generatedIdeas.Replace("```json", "").Replace("```", "").Trim();
            var ideasResponse = JsonSerializer.Deserialize<GeneratedIdeasResponse>(cleanJson);

            return ideasResponse?.Ideas;
        }
        catch (JsonException ex)
        {
            return null;
        }
    }

    private static string[] GetMembersFullNamesAsync(string generatedIdeas)
    {
        try
        {
            var cleanJson = generatedIdeas.Replace("```json", "").Replace("```", "").Trim();
            var membersResponse = JsonSerializer.Deserialize<ExtractedMembersResponse>(cleanJson);

            return membersResponse?.Members;
        }
        catch (JsonException ex)
        {
            return null;
        }
    }
}
