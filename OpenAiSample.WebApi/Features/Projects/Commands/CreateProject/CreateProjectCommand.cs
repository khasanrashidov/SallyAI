using MediatR;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Projects.Commands.CreateProject;

public class CreateProjectCommand : IRequest<Project>
{
    public string Name { get; set; }

    public string ClientName { get; set; }

    public int TechLeadId { get; set; }

    public string Description { get; set; }

    public int? MonthsNeeded { get; set; }

    public IFormFile Transcript { get; set; }

    public IFormFile Summary { get; set; }
}
