using RankingUp.Core.Data;
using RankingUp.Tournament.Domain.Entities;

namespace RankingUp.Tournament.Domain.Repositories
{
    public interface ITournamentTeamRepository : IGetRepository<TournamentTeam>, IInsertRepository<TournamentTeam>, IDeleteRepository<TournamentTeam>
    {
        Task<IEnumerable<TournamentTeam>> GetAllByTournament(Guid Id);
    }
}
