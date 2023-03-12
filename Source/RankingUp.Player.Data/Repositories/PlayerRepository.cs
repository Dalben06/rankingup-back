using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;
using RankingUp.Core.Extensions;
using RankingUp.Player.Domain.Entities;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Player.Data.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IBaseRepository _baseRepository;

        public PlayerRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }
        private string GetDefaultSql()
        {
            return @"
             SELECT 
                Players.*,
                Sports.*,
                Clubs.*
             FROM Players

             LEFT JOIN PlayerClubs
             ON Players.Id = PlayerClubs.PlayerId

             LEFT JOIN PlayerSports
             ON Players.Id = PlayerSports.PlayerId

             LEFT JOIN Sports
             ON Sports.Id = PlayerSports.SportId

             LEFT JOIN Clubs
             ON Clubs.Id = PlayerClubs.ClubId

             WHERE 1 = 1
             AND Players.IsDeleted = 0 
             ";
        }

        public Task<IEnumerable<Players>> GetAll() => _baseRepository.GetAsync<Players, Sports, Clubs>(GetDefaultSql(),SQLMap());
        public Task<Players> GetById(Guid Id) => _baseRepository.GetByIdAsync<Players, Sports, Clubs>(GetDefaultSql(), Id, SQLMap());
        public Task<Players> GetById(int Id) => _baseRepository.GetByIdAsync<Players, Sports, Clubs>(GetDefaultSql(), Id, SQLMap());
        public Task<Players> InsertAsync(Players entity) => _baseRepository.InsertAsync<Players>(entity);
        public Task<bool> UpdateAsync(Players entity) => _baseRepository.UpdateAsync<Players>(entity);
        public Task<bool> DeleteAsync(Players entity) => _baseRepository.DeleteAsync<Players>(entity);
        private Func<Players, Sports , Clubs, Players> SQLMap()
        {
            var dic = new Dictionary<long, Players>();
            return (player, Sport,Clubs) =>
            {
                if (dic.TryGetValue(player.Id, out Players existingplayer))
                    player = existingplayer;
                else
                    dic.Add(player.Id, player);

                if (player.Sports == null) player.Sports = new List<Sports>();

                if (player.Clubs == null) player.Clubs = new List<Clubs>();

                player.Sports.Add(Sport);
                player.Clubs.Add(Clubs);
                return player;
            };

        }

    }
}
