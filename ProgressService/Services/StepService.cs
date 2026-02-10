using ProgressService.Models;
using ProgressService.Models.Dto;
using ProgressService.Repositories.Interfaces;
using ProgressService.Services.Interfaces;

namespace ProgressService.Services
{
    public class StepService : IStepService
    {
        private readonly IStepRepository _stepRepository;

        public StepService(IStepRepository stepRepository)
        {
            _stepRepository = stepRepository;
        }
        public async Task<List<StepModel>> GetAllSteps() => await _stepRepository.GetAllSteps();

    }
}
