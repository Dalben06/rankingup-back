using RankingUp.Core.Data;
using RankingUp.Tournament.Domain.Entities;

namespace RankingUp.Tournament.Domain.Repositories
{
    public interface ITournamentsRepository : IInsertRepository<Tournaments>, IUpdateRepository<Tournaments>
        , IDeleteRepository<Tournaments>, IGetRepository<Tournaments>
    {
        Task<IEnumerable<Tournaments>> GetAll();

    }
}
