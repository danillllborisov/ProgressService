using ProgressService.Models;
using ProgressService.Models.Dto;

namespace ProgressService.Services.Interfaces
{
    public interface IStepService
    {
        Task<List<StepModel>> GetAllSteps();

    }
}
