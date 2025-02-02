using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenAiSample.WebApi.Features.Projects.Commands.CreateProject;
using OpenAiSample.WebApi.Features.Projects.Queries.GetAllProjects;
using OpenAiSample.WebApi.Features.Projects.Queries.GetProjectById;
using OpenAiSample.WebApi.Services.Identity;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Projects;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Route("projects")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;


    public ProjectsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(currentUserService);

        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken = default)
    {
        var query = new GetAllProjectsQuery();
        var projects = await _mediator.Send(query, cancellationToken);

        return Ok(projects);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetProjectByIdQuery() { Id = id };
        var project = await _mediator.Send(query, cancellationToken);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProject([FromForm] CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }
}
