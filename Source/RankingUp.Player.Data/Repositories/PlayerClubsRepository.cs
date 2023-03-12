using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Player.Domain.IRepositories;

namespace RankingUp.Player.Data.Repositories
{
    public class PlayerClubsRepository : IPlayerClubsRepository
    {

        private readonly IBaseRepository _baseRepository;

        public PlayerClubsRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }
        private string GetDefaultSql()
        {
            return $@"
             SELECT 

             PlayerClubs.*

             {FROM()}
             ";
        }

        private string FROM()
        {
            return @"
             FROM PlayerClubs

             INNER JOIN Players
             ON Players.Id = PlayerClubs.PlayerId
             AND Players.IsDeleted = 0 

             INNER JOIN Clubs
             ON Clubs.Id = PlayerClubs.ClubId
             AND Clubs.IsDeleted = 0

             WHERE 1 = 1
             AND PlayerClubs.IsDeleted = 0

            ";
        }


        public Task<PlayerClubs> GetById(Guid Id) => _baseRepository.GetByIdAsync<PlayerClubs>(GetDefaultSql(), Id);
        public Task<PlayerClubs> GetById(int Id) => _baseRepository.GetByIdAsync<PlayerClubs>(GetDefaultSql(), Id);
        public Task<IEnumerable<PlayerClubs>> GetPlayerFromClub(Guid Id) => _baseRepository.GetAsync<PlayerClubs>(GetDefaultSql() + " AND  Clubs.UUId = @Id ", new { Id });
        public Task<PlayerClubs> InsertAsync(PlayerClubs entity) => _baseRepository.InsertAsync<PlayerClubs>(entity);
        public Task<bool> UpdateAsync(PlayerClubs entity) => _baseRepository.UpdateAsync<PlayerClubs>(entity);
        public Task<bool> DeleteAsync(PlayerClubs entity) => _baseRepository.DeleteAsync<PlayerClubs>(entity);

        public Task<IEnumerable<Clubs>> GetClubsFromPlayer(Guid Id)
        {
            var SQL = $@" SELECT Clubs.*  {FROM()} AND Players.UUId = @Id";
            return _baseRepository.GetAsync<Clubs>(SQL, new { Id });
        }

        public Task<IEnumerable<Players>> GetPlayersFromClub(Guid Id)
        {
            var SQL = $@" SELECT players.*  {FROM()} AND Clubs.UUId = @Id";
            return _baseRepository.GetAsync<Players>(SQL, new { Id });
        }

        public async Task<PlayerClubs> GetPlayersAndClubId(Guid PlayerId, Guid ClubId)
        {
            var result = await _baseRepository.GetAsync<PlayerClubs>(GetDefaultSql() + " AND Clubs.UUId = @ClubId AND Players.UUId = @PlayerId", new { PlayerId, ClubId });
            return result == null ? null : result.FirstOrDefault();
        }
    }
}
