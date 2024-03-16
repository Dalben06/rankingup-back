using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.ViewModels;

namespace RankingUp.Tournament.Application.Interfaces
{
    public interface IRankingGameService
    {
        Task<RequestResponse<IEnumerable<RankingGameDetailViewModel>>> GetGames(Guid RankingId, bool? IsFinished);
        Task<RequestResponse<IEnumerable<RankingTeamViewModel>>> GetRankingPlayers(Guid RankingId);
        Task<RequestResponse<RankingGameDetailViewModel>> CreateGameUsingQueue(Guid TournamentId, int UseId);
        Task<RequestResponse<RankingGameDetailViewModel>> CreateGame(RankingCreateGameViewModel model);
        Task<RequestResponse<RankingGameDetailViewModel>> UpdateGame(RankingGameDetailViewModel model);

    }
}
