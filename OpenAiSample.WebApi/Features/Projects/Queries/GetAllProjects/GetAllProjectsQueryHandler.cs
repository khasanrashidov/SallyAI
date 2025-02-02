using MediatR;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Repositories;

namespace OpenAiSample.WebApi.Features.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
    {
        private readonly IProjectsRepository _projectsRepository;

        public GetAllProjectsQueryHandler(IProjectsRepository projectsRepository)
        {
            ArgumentNullException.ThrowIfNull(projectsRepository);

            _projectsRepository = projectsRepository;
        }

        public async Task<IEnumerable<Project>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectsRepository.GetProjectsAsync(cancellationToken);

            return projects;
        }
    }
}
