using MediatR;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<Project>
{
    public int Id { get; set; }
}
