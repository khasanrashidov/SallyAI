using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAiSample.WebApi.Services.Identity;
using OpenApiSample.Data;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Repositories;

namespace OpenAiSample.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly AppDbContext _context;

        public ProjectController(ICurrentUserService currentUserService, UserManager<User> userManager, IProjectRepository projectRepository, AppDbContext context)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _projectRepository = projectRepository;
            _context = context;
        }

        [HttpGet("user-projects")]
        public async Task<IActionResult> GetAllUserProjects()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(
                u => u.Email == _currentUserService.UserEmail!
                );

            var userProjects = await _context.UserProjects
        .Where(up => up.UserId == user.Id)
        .Include(up => up.Project)
            .ThenInclude(p => p.Members)  // Include Members of the project
        .Include(up => up.Project)
            .ThenInclude(p => p.Ideas)    // Include Ideas of the project
        .Select(up => up.Project)       // Select the Project part of the UserProject
        .ToListAsync();

            return Ok(userProjects);
        }

        // Change state of Project using a projectId and stateNumber
        [HttpPut("change-state")]
        public async Task<IActionResult> ChangeState(int projectId, int state)
        {
            var project = await _projectRepository.GetAsync(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            project.State = state;

            await _projectRepository.UpdateAsync(project);

            return Ok(project);
        }
    }
}
