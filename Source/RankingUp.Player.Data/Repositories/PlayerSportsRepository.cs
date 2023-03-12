using MySqlX.XDevAPI.Common;
using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Player.Data.Repositories
{
    public class PlayerSportsRepository : IPlayerSportsRepository
    {
        private readonly IBaseRepository _baseRepository;

        public PlayerSportsRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return $@"
             SELECT 

             PlayerSports.*

             {FROM()}
             ";
        }

        private string FROM()
        {
            return @"
             FROM PlayerSports

             INNER JOIN Players
             ON Players.Id = PlayerSports.PlayerId
             AND Players.IsDeleted = 0 

             INNER JOIN Sports
             ON Sports.Id = PlayerSports.SportId

             WHERE 1 = 1
             AND PlayerSports.IsDeleted = 0

            ";
        }

        public Task<PlayerSports> GetById(Guid Id) => _baseRepository.GetByIdAsync<PlayerSports>(GetDefaultSql(), Id);
        public Task<PlayerSports> GetById(int Id) => _baseRepository.GetByIdAsync<PlayerSports>(GetDefaultSql(), Id);
        public Task<PlayerSports> InsertAsync(PlayerSports entity) => _baseRepository.InsertAsync<PlayerSports>(entity);
        public Task<bool> UpdateAsync(PlayerSports entity) => _baseRepository.UpdateAsync<PlayerSports>(entity);
        public Task<bool> DeleteAsync(PlayerSports entity) => _baseRepository.DeleteAsync<PlayerSports>(entity);

        public Task<IEnumerable<Sports>> GetSportsFromPlayer(Guid Id)
        {
            var SQL = $@" SELECT Sports.*  {FROM()} AND Players.UUId = @Id";
            return _baseRepository.GetAsync<Sports>(SQL, new { Id });
        }

        public async Task<PlayerSports> GetPlayerAndSportId(Guid PlayerId, Guid SportId)
        {
            var result = await _baseRepository.GetAsync<PlayerSports>(GetDefaultSql() + " AND Players.UUId = @PlayerId AND  Sports.UUId = @SportId", new { PlayerId, SportId });
            return result == null ? null : result.FirstOrDefault();

        }
    }
}
