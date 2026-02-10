using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressService.Models.Dto;
using ProgressService.Services.Interfaces;

namespace ProgressService.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class StepController : ControllerBase

    {
        private readonly IStepService _stepService;
        public StepController(IStepService stepService) => _stepService = stepService;

        [HttpGet("all/steps")]
        public async Task<IActionResult> GetAllProjectsByAdmin()
        {
            var steps = await _stepService.GetAllSteps();
            return Ok(steps);
        }
       
    }
}
