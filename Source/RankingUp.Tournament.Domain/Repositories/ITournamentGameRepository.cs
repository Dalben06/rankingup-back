using RankingUp.Core.Data;
using RankingUp.Tournament.Domain.Entities;

namespace RankingUp.Tournament.Domain.Repositories
{
    public interface ITournamentGameRepository : IGetRepository<TournamentGame>, IUpdateRepository<TournamentGame>, IInsertRepository<TournamentGame>, IDeleteRepository<TournamentGame>
    {

        Task<IEnumerable<TournamentGame>> GetAllGamesByTournamentId(Guid Id);

        Task<TournamentGame> GetGameNotFinishByTeamAndTournamentId(Guid tour, Guid teamId);
        Task<IEnumerable<TournamentGame>> GetGameNotFinishTournamentId(Guid tour);

    }
}
