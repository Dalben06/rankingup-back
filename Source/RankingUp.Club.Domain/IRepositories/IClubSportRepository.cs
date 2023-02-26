using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;

namespace RankingUp.Club.Domain.IRepositories
{
    public interface IClubSportRepository : IInsertRepository<ClubSport>, IUpdateRepository<ClubSport>, IDeleteRepository<ClubSport>, IGetRepository<ClubSport>
    {
        Task<IEnumerable<ClubSport>> GetSportFromClubId(Guid clubId);
    }
}
