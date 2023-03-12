using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Player.Domain.IRepositories
{
    public interface IPlayerSportsRepository : IInsertRepository<PlayerSports>, IUpdateRepository<PlayerSports>
        , IDeleteRepository<PlayerSports>, IGetRepository<PlayerSports>
    {
        Task<IEnumerable<Sports>> GetSportsFromPlayer(Guid Id);
        Task<PlayerSports> GetPlayerAndSportId(Guid PlayerId, Guid SportId);

    }
}
