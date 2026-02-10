using ProgressService.Models;
using ProgressService.Models.Dto;

namespace ProgressService.Repositories.Interfaces
{
    public interface IStepRepository
    {
        Task<List<StepModel>> GetAllSteps();

    }
}
