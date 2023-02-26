using RankingUp.Club.Domain.Entities;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Data;

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
             SELECT CS.Id, CS.ClubId, CS.SportId
             FROM ClubSports CS

             JOIN Clubs C
             ON C.Id = CS.ClubId
            
             WHERE 1 = 1

            ";
        }


        public Task<ClubSport> GetById(Guid Id) => this._baseRepository.GetByIdAsync<ClubSport>(Id);
        public Task<ClubSport> GetById(int Id) => this._baseRepository.GetByIdAsync<ClubSport>(Id);
        public Task<ClubSport> InsertAsync(ClubSport club) => this._baseRepository.InsertAsync<ClubSport>(club);
        public Task<bool> UpdateAsync(ClubSport entity) => this._baseRepository.UpdateAsync<ClubSport>(entity);
        public Task<bool> DeleteAsync(ClubSport entity) => this._baseRepository.DeleteAsync<ClubSport>(entity);

        public Task<IEnumerable<ClubSport>> GetSportFromClubId(Guid clubId) => this._baseRepository.GetAsync<ClubSport>(" AND C.UUId = @clubId", new { clubId });
    }
}
