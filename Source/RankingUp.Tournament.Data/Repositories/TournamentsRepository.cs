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
             AND Tournaments.IsDeleted = FALSE 


             ";
        }


        private static string GetDistanceSql()
        {
            return @"(
                3959 *
                acos(
                    least(1, greatest(-1,
                        cos(radians(@Latitude)) *
                        cos(radians(Tournaments.latitude)) *
                        cos(radians(Tournaments.longitude) -
                        radians(@Longitude)) +
                        sin(radians(@Latitude)) *
                        sin(radians(Tournaments.latitude))
                    ))
                )
            )";
        }

        private string GetDefaultPaginationSql(TournamentFilter filter)
        {
            var distance = filter.Latitude.HasValue && filter.Longitude.HasValue
                ? $"{GetDistanceSql()} AS distance"
                : "NULL AS distance";

            return @$"
             SELECT Tournaments.* 
               ,{distance},
              Clubs.*
              FROM Tournaments

             INNER JOIN Clubs
             ON Clubs.Id = Tournaments.ClubId

             WHERE 1 = 1
             AND Tournaments.IsDeleted = FALSE 

             ";
        }

        public Task<IEnumerable<Tournaments>> GetAll(bool? isRanking) 
        { 
            var sql = GetDefaultSql();

            if (isRanking.HasValue)
                sql += " AND Tournaments.IsRanking = @isRanking";

            return _baseRepository.GetAsync<Tournaments, Clubs>(sql, SQLMap(), new { isRanking });
        }
        public Task<IEnumerable<Tournaments>> GetAllByClub(Guid id)
        {
            var sql = GetDefaultSql() + " AND Clubs.UUId = @id";

            return _baseRepository.GetAsync<Tournaments, Clubs>(sql, SQLMap(), new { id });
        }

        private static DateTime? AsUtc(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return value.Value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.Value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            };
        }

        public Task<Pagination<Tournaments>> GetTournamentsByFilter(TournamentFilter filter)
        {
            filter.StartDate = AsUtc(filter.StartDate);
            filter.EndDate = AsUtc(filter.EndDate);

            var sql = GetDefaultPaginationSql(filter);

            sql += @$" AND Tournaments.IsRanking = @IsRanking";

            if(filter.OnlyFinished.HasValue)
                sql += filter.OnlyFinished.Value
                    ? " AND Tournaments.FinishDate IS NOT NULL "
                    : " AND Tournaments.FinishDate IS NULL ";

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                sql += @" AND Tournaments.EventDate BETWEEN @StartDate AND @EndDate ";

            if (filter.RadioDistance.HasValue && filter.Latitude.HasValue && filter.Longitude.HasValue)
                sql += $" AND {GetDistanceSql()} <= @RadioDistance ";

            var orderColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = "Tournaments.Id",
                ["UUId"] = "Tournaments.UUId",
                ["Name"] = "Tournaments.Name",
                ["EventDate"] = "Tournaments.EventDate",
                ["EventHourStart"] = "Tournaments.EventHourStart",
                ["EventHourEnd"] = "Tournaments.EventHourEnd",
                ["StartDate"] = "Tournaments.StartDate",
                ["FinishDate"] = "Tournaments.FinishDate",
                ["CreateDate"] = "Tournaments.CreateDate",
                ["UpdateDate"] = "Tournaments.UpdateDate",
                ["Distance"] = "distance"
            };

            var orderType = string.Equals(filter.OrderType, "ASC", StringComparison.OrdinalIgnoreCase)
                ? "ASC"
                : string.Equals(filter.OrderType, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : null;

            if (orderType != null && filter.Order != null && orderColumns.TryGetValue(filter.Order, out var orderColumn))
                sql += $" ORDER BY {orderColumn} {orderType} ";
            else
                sql += " ORDER BY Tournaments.Id DESC ";

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
