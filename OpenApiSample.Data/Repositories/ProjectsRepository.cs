using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.EntityFrameworkCore;
using OpenApiSample.Data.Entities;

namespace OpenApiSample.Data.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly AppDbContext _context;

    public ProjectsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return await _context.Projects.ToListAsync(cancellationToken);
    }

    public async Task<Project> GetProjectAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return project;
    }

    public async Task<Project> UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
