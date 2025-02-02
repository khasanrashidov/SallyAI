using MediatR;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Projects.Queries.GetAllProjects;

public class GetAllProjectsQuery : IRequest<IEnumerable<Project>>
{
}
