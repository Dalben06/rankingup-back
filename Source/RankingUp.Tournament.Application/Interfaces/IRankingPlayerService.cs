using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.ViewModels;

namespace RankingUp.Tournament.Application.Interfaces
{
    public interface IRankingPlayerService
    {

        Task<RequestResponse<IEnumerable<RankingPlayerViewModel>>> GetPlayers(Guid RankingId);
        Task<RequestResponse<IEnumerable<RankingPlayerQueueViewModel>>> GetPlayersOnQueue(Guid RankingId);

        Task<RequestResponse<RankingPlayerViewModel>> AddPlayerQuickly(RankingAddPlayerQuicklyViewModel model);
        Task<RequestResponse<RankingPlayerViewModel>> AddPlayer(RankingPlayerViewModel model);
        Task<NoContentResponse> RemovePlayer(Guid Id, int UseId);
    }
}
