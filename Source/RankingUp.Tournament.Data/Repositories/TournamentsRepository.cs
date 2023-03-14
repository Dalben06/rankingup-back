using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;
using RankingUp.Sport.Domain.Entities;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;

namespace RankingUp.Tournament.Data.Repositories
{
    public class TournamentsRepository : ITournamentsRepository
    {
        private readonly IBaseRepository _baseRepository;

        public TournamentsRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @"
             SELECT Tournaments.*, Clubs.*
             FROM Tournaments

             INNER JOIN Clubs
             ON Clubs.Id = Tournaments.ClubId

             WHERE 1 = 1
             AND Tournaments.IsDeleted = 0 


             ";
        }

        public Task<IEnumerable<Tournaments>> GetAll(bool? isRanking) 
        { 
            var sql = GetDefaultSql();

            if (isRanking.HasValue)
                sql += " AND Tournaments.IsRanking = @isRanking";

            return _baseRepository.GetAsync<Tournaments, Clubs>(GetDefaultSql(), SQLMap(), new { isRanking });
        }
        public Task<Tournaments> GetById(Guid Id) => _baseRepository.GetByIdAsync<Tournaments, Clubs>(GetDefaultSql(), Id, SQLMap());
        public Task<Tournaments> GetById(int Id) => _baseRepository.GetByIdAsync<Tournaments, Clubs>(GetDefaultSql(), Id, SQLMap());
        public Task<Tournaments> InsertAsync(Tournaments entity) => _baseRepository.InsertAsync<Tournaments>(entity);
        public Task<bool> UpdateAsync(Tournaments entity) => _baseRepository.UpdateAsync<Tournaments>(entity);
        public Task<bool> DeleteAsync(Tournaments entity) => _baseRepository.DeleteAsync<Tournaments>(entity);

        private Func<Tournaments, Clubs, Tournaments> SQLMap()
        {
            var dic = new Dictionary<long, Tournaments>();
            return (@event, club) =>
            {
                if (dic.TryGetValue(@event.Id, out var existingEvent))
                {
                    @event = existingEvent;
                }
                else
                {
                    @event.SetClub(club);
                    dic.Add(@event.Id, @event);
                }

                return @event;
            };

        }
    }
}
