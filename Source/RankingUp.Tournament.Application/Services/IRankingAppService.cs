using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.ViewModels;

namespace RankingUp.Tournament.Application.Services
{
    public interface IRankingAppService
    {
        Task<RequestResponse<IEnumerable<RankingDetailViewModel>>> GetAllRankings();
        Task<RequestResponse<RankingDetailViewModel>> GetRanking(Guid Id);
        Task<RequestResponse<IEnumerable<RankingPlayerViewModel>>> GetPlayers(Guid RankingId);
        Task<RequestResponse<RankingDetailViewModel>> CreateRanking(RankingDetailViewModel model);
        Task<RequestResponse<RankingDetailViewModel>> UpdateRanking(RankingDetailViewModel model);
        Task<RequestResponse<RankingPlayerViewModel>> AddPlayer(RankingPlayerViewModel model);
        Task<NoContentResponse> RemovePlayer(Guid Id, int UseId);
        Task<NoContentResponse> StartRanking(Guid Id, int UseId);
        Task<NoContentResponse> EndRanking(Guid Id, int UseId);
        Task<NoContentResponse> RemoveRanking(Guid Id, int UseId);
    }
}
