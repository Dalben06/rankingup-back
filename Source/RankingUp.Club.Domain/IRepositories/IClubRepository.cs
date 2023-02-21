using RankingUp.Core.Data;

namespace RankingUp.Club.Domain.IRepositories
{
    public interface IClubRepository : IInsertRepository<Entities.Club> , IUpdateRepository<Entities.Club>, IDeleteRepository<Entities.Club>, IGetRepository<Entities.Club>
    {
        


    }
}
