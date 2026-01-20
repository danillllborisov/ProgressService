using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProgressService.Models.Dto;
using ProgressService.Services.Interfaces;

namespace ProgressService.Controllers
{
    [ApiController]
    [Route("api/project")]
    [Authorize(Policy = "AdminOnly")]
    public class ProjectsController : ControllerBase

    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService) => _projectService = projectService;

        [HttpPost("admin/{adminId:int}")]
        public async Task<IActionResult> CreateProject(
            [FromRoute] int adminId,
            [FromBody] CreateProjectRequest dto)
        {
            var projectId = await _projectService.CreateProjectAsync(adminId, dto);

            return CreatedAtAction(
                nameof(GetProjectById),
                new { projectId },
                new { projectId }
            );
        }

        [HttpGet("all/{adminId:int}")]
        public async Task<IActionResult> GetAllProjectsByAdmin(int adminId)
        {
            var projects = await _projectService.GetAllProjectsByAdminAsync(adminId);
            return Ok(projects);
        }

        [HttpGet("project/{projectId:int}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var projects = await _projectService.GetOneProjectAsync(projectId);
            return Ok(projects);
        }

        [AllowAnonymous]
        [HttpGet("project/{link}")]
        public async Task<IActionResult> GetProjectByLink(string link)
        {
            var projects = await _projectService.GetProjectByLinkAsync(link);
            return Ok(projects);
        }

        [HttpPut("updateProject/{projectId:int}")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody] UpdateProjectRequest dto)
        {
            if (dto is null)
                return BadRequest("Request body is required.");

            var updated = await _projectService.UpdateProjectAsync(projectId, dto);

            if (!updated)
                return NotFound(); 

            return NoContent();
        }


    }
}
