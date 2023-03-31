using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Data;

namespace RankingUp.Club.Domain.IRepositories
{
    public interface IClubRepository : IInsertRepository<Clubs> , IUpdateRepository<Clubs>, IDeleteRepository<Clubs>, IGetRepository<Clubs>
    {

        Task<IEnumerable<Clubs>> GetClubsBySportId(Guid Id);
        Task<IEnumerable<Clubs>> GetClubs(Guid[] Id);
        Task<IEnumerable<Clubs>> GetClubAndSportId(Guid ClubId, Guid SportId);

        Task<IEnumerable<Clubs>> GetAll();


    }
}
