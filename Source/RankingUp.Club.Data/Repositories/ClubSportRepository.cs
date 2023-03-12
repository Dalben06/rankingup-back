using RankingUp.Club.Domain.Entities;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Data;
using RankingUp.Core.Extensions;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Club.Data.Repositories
{
    public class ClubSportRepository : IClubSportRepository
    {
        private readonly IBaseRepository _baseRepository;
        public ClubSportRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @" 
             SELECT 
                ClubSports.*,
                Clubs.*,
                Sports.*
             FROM ClubSports

             JOIN Clubs 
             ON Clubs.Id = ClubSports.ClubId

             JOIN Sports 
             ON Sports.Id = ClubSports.SportId
            
             WHERE 1 = 1

            ";
        }


        public Task<ClubSport> GetById(Guid Id) => this._baseRepository.GetByIdAsync<ClubSport, Clubs, Sports>(GetDefaultSql(), Id, SQLMap());
        public Task<ClubSport> GetById(int Id) => this._baseRepository.GetByIdAsync<ClubSport, Clubs, Sports>(GetDefaultSql(), Id, SQLMap());
        public Task<ClubSport> InsertAsync(ClubSport club) => this._baseRepository.InsertAsync<ClubSport>(club);
        public Task<bool> UpdateAsync(ClubSport entity) => this._baseRepository.UpdateAsync<ClubSport>(entity);
        public Task<bool> DeleteAsync(ClubSport entity) => this._baseRepository.DeleteAsync<ClubSport>(entity);
        public Task<IEnumerable<ClubSport>> GetSportFromClubId(Guid clubId) 
            => this._baseRepository.GetAsync<ClubSport, Clubs, Sports>(GetDefaultSql() + " AND Clubs.UUId = @clubId", SQLMap(), new { clubId });


        private Func<ClubSport, Clubs, Sports, ClubSport> SQLMap()
        {
            var dic = new Dictionary<long, ClubSport>();
            return (ClubSport, Club, Sport) =>
            {
                if (dic.TryGetValue(ClubSport.Id, out ClubSport existingClub))
                    ClubSport = existingClub;
                else
                {
                    ClubSport.Club = Club;
                    ClubSport.Sport = Sport;
                    dic.Add(ClubSport.Id, ClubSport);
                }
                return ClubSport;
            };

        }
    }
}
