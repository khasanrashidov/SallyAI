using OpenApiSample.Data.Entities;

namespace OpenApiSample.Data.Repositories
{
    public class ProjectRepository(AppDbContext context) : BaseRepository<Project>(context), IProjectRepository
    {
    }
}
