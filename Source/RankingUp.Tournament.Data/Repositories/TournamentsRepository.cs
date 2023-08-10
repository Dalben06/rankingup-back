using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;
using RankingUp.Core.Domain;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Entities.Filters;
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


        private string GetDefaultPaginationSql()
        {
            return @"
             SELECT Tournaments.* 
               ,(
                   3959 *
                   acos(cos(radians(37)) * 
                   cos(radians(Tournaments.latitude)) * 
                   cos(radians(Tournaments.longitude) - 
                   radians(-122)) + 
                   sin(radians(37)) * 
                   sin(radians(Tournaments.latitude )))
                ) AS distance,
              Clubs.*
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
        public Task<IEnumerable<Tournaments>> GetAllByClub(Guid id)
        {
            var sql = GetDefaultSql() + " AND Clubs.Id = @id";

            return _baseRepository.GetAsync<Tournaments, Clubs>(GetDefaultSql(), SQLMap(), new { id });
        }

        public Task<Pagination<Tournaments>> GetTournamentsByFilter(TournamentFilter filter)
        {
            var sql = GetDefaultPaginationSql();

            sql += @$" AND Tournaments.IsRanking = @IsRanking";

            if(filter.OnlyFinished.HasValue)
                sql += @$" AND Tournaments.FinishDate {(filter.OnlyFinished.Value ? " NOT ": "")} is null ";

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                sql += @$" AND Tournaments.EventHourStart BETWEEN @StartDate AND @EndDate ";

            //if (filter.RadioDistance.HasValue)
            //    sql += @" AND Tournaments.latitude = @latitude AND Tournaments.longitude = @latitude AND ";

            if(!string.IsNullOrEmpty(filter.Order) && !string.IsNullOrEmpty(filter.OrderType))
                sql += $"  ORDER BY tournaments.{filter.Order} {filter.OrderType} ";

            return _baseRepository.GetPagination<Tournaments,Clubs>(filter, sql,SQLMap(), param: filter);
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
