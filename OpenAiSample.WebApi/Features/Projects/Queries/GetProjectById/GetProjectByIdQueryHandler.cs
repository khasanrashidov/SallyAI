using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenApiSample.Data;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Project>
{
    private readonly AppDbContext _context;

    public GetProjectByIdQueryHandler(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        //var project = await _projectRepository.GetAsync(predicate: p => p.Id == request.Id, include: p => p.Include(p => p.Ideas).Include(p => p.Members));

        var project = await _context.Projects
            .Include(p => p.Ideas)
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        if (project == null)
        {
            return null;
        }

        return project;
    }
}
