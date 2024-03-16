using RankingUp.Core.Domain;
using RankingUp.Core.ViewModels;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities.Filters;

namespace RankingUp.Tournament.Application.Interfaces
{
    public interface IRankingAppService
    {
        Task<RequestResponse<IEnumerable<RankingDetailViewModel>>> GetAllRankings();
        Task<RequestResponse<PaginationViewModel<RankingDetailViewModel>>> GetRankingByFilter(TournamentFilter filter);
        Task<RequestResponse<RankingDetailViewModel>> GetRanking(Guid Id);
        Task<RequestResponse<IEnumerable<RankingDetailViewModel>>> GetRankingsByClub(Guid ClubId);
        Task<RequestResponse<RankingDetailViewModel>> CreateRanking(RankingDetailViewModel model);
        Task<NoContentResponse> StartRanking(Guid Id, int UseId);
        Task<NoContentResponse> EndRanking(Guid Id, int UseId);
        Task<RequestResponse<RankingDetailViewModel>> UpdateRanking(RankingDetailViewModel model);
        Task<NoContentResponse> RemoveRanking(Guid Id, int UseId);
    }
}
