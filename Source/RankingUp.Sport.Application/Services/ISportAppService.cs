using RankingUp.Core.Domain;
using RankingUp.Sport.Application.ViewModels;

namespace RankingUp.Sport.Application.Services
{
    public interface ISportAppService
    {

        Task<RequestResponse<IEnumerable<SportViewModel>>> GetAllSports();
        Task<RequestResponse<SportViewModel>> GetSportById(Guid Id);

    }
}
