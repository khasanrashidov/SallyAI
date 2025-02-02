
using OpenApiSample.Data.Entities;

namespace OpenApiSample.Data.Repositories;

public interface IProjectsRepository
{
    Task<IEnumerable<Project>> GetProjectsAsync(CancellationToken cancellationToken);

    Task<Project> GetProjectAsync(int id, CancellationToken cancellationToken);

    Task<Project> CreateProjectAsync(Project project);

    Task<Project> UpdateProjectAsync(Project project);

    Task DeleteProjectAsync(int id);
}
