using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;

namespace RankingUp.Tournament.Data.Repositories
{
    public class TournamentTeamRepository : ITournamentTeamRepository
    {

        private readonly IBaseRepository _baseRepository;

        public TournamentTeamRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        // Todo verificar quando tiver o Times
        private string GetDefaultSql()
        {
            return @"
             SELECT TournamentTeams.*, Tournaments.*,Players.*
             FROM TournamentTeams

             INNER JOIN Tournaments
             ON Tournaments.Id = TournamentTeams.TournamentId

             INNER JOIN Players
             ON Players.Id = TournamentTeams.TeamId

             WHERE 1 = 1
             AND TournamentTeams.IsDeleted = 0
             AND Tournaments.IsDeleted = 0 
             AND Players.IsDeleted = 0 
             AND TournamentTeams.IsActive = 1
             ";
        }

        public Task<IEnumerable<TournamentTeam>> GetAllByTournament(Guid Id) 
            => _baseRepository.GetAsync<TournamentTeam, Tournaments, Players>(GetDefaultSql() + " AND Tournaments.UUId = @Id", SQLMapPlayer(), new {Id});
        public Task<TournamentTeam> GetById(Guid Id) => _baseRepository.GetByIdAsync<TournamentTeam, Tournaments, Players>(GetDefaultSql(), Id, SQLMapPlayer());
        public Task<TournamentTeam> GetById(int Id) => _baseRepository.GetByIdAsync<TournamentTeam, Tournaments, Players>(GetDefaultSql(), Id, SQLMapPlayer());
        public Task<TournamentTeam> InsertAsync(TournamentTeam entity) => _baseRepository.InsertAsync<TournamentTeam>(entity);
        public Task<bool> UpdateAsync(TournamentTeam entity) => _baseRepository.UpdateAsync<TournamentTeam>(entity);
        public Task<bool> DeleteAsync(TournamentTeam entity) => _baseRepository.DeleteAsync<TournamentTeam>(entity);

        private Func<TournamentTeam, Tournaments,Players, TournamentTeam> SQLMapPlayer()
        {
            var dic = new Dictionary<long, TournamentTeam>();
            return (team, Tour, player) =>
            {
                if (dic.TryGetValue(team.Id, out var existingTeam))
                {
                    team = existingTeam;
                }
                else
                {
                    team.SetTournament(Tour);
                    team.SetTeam(player);
                    dic.Add(team.Id, team);
                }

                return team;
            };

        }

       
    }
}
