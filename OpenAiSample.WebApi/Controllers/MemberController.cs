using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApiSample.Data.Repositories;

namespace OpenAiSample.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;

        public MemberController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _memberRepository.GetAllAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var member = await _memberRepository.GetAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        [HttpGet("ByProject/{projectId}")]
        public async Task<IActionResult> GetByProjectId(int projectId)
        {
            var members = await _memberRepository.GetAllAsync(m => m.ProjectId == projectId);

            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Member member)
        {
            if (member == null)
            {
                return BadRequest();
            }

            await _memberRepository.AddAsync(member);
            return CreatedAtAction(nameof(Get), new { id = member.Id }, member);
        }
    }
}
