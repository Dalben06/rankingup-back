using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;

namespace RankingUp.Player.Domain.IRepositories
{
    public interface IPlayerClubsRepository : IInsertRepository<PlayerClubs>, IUpdateRepository<PlayerClubs>
        , IDeleteRepository<PlayerClubs>, IGetRepository<PlayerClubs>
    {
        Task<IEnumerable<Clubs>> GetClubsFromPlayer(Guid Id);
        Task<IEnumerable<Players>> GetPlayersFromClub(Guid Id);
        Task<Players> GetPlayerAndClubId(int ClubId, int PlayerId);
        Task<PlayerClubs> GetPlayersAndClubId(Guid PlayerId, Guid ClubId);

    }
}
