using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;

namespace RankingUp.Tournament.Data.Repositories
{
    public class RankingQueueRepository : IRankingQueueRepository
    {
        private readonly IBaseRepository _baseRepository;

        public RankingQueueRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @"
             SELECT RankingQueues.*,Tournaments.*,TournamentTeams.*,Players.*
             FROM RankingQueues

             INNER JOIN Tournaments
             ON Tournaments.Id = RankingQueues.TournamentId

             INNER JOIN TournamentTeams
             ON TournamentTeams.Id = RankingQueues.TeamId

             INNER JOIN Players
             ON Players.Id = TournamentTeams.TeamId

             WHERE 1 = 1
             AND RankingQueues.IsDeleted = 0
             AND Tournaments.IsDeleted = 0 
             AND TournamentTeams.IsDeleted = 0 
             AND Players.IsDeleted = 0 
             AND Tournaments.FinishDate is null
             ";
        }

        
        public Task<IEnumerable<RankingQueue>> GetByTournamentIdOrderByCreateDate(Guid Id)
            => _baseRepository.GetAsync<RankingQueue, Tournaments, TournamentTeam, Players>(GetDefaultSql() + " AND Tournaments.UUId = @Id ORDER BY RankingQueues.CreateDate ASC", SQLMapPlayer(), new { Id});

        public Task<RankingQueue> InsertAsync(RankingQueue entity) => _baseRepository.InsertAsync<RankingQueue>(entity);

        public Task<bool> DeleteAsync(RankingQueue entity) => _baseRepository.DeleteAsync<RankingQueue>(entity);

        private Func<RankingQueue, Tournaments, TournamentTeam,Players, RankingQueue> SQLMapPlayer()
        {
            var dic = new Dictionary<long, RankingQueue>();
            return (queue, Tour,team,  player) =>
            {
                if (dic.TryGetValue(team.Id, out var existingqueue))
                {
                    queue = existingqueue;
                }
                else
                {
                    queue.SetTournament(Tour);
                    team.SetTeam(player);
                    queue.SetTeam(team);
                    dic.Add(queue.Id, queue);
                }

                return queue;
            };

        }

    }
}
