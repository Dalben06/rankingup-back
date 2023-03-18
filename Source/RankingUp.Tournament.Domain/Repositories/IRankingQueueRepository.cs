using RankingUp.Core.Data;
using RankingUp.Tournament.Domain.Entities;

namespace RankingUp.Tournament.Domain.Repositories
{
    public interface IRankingQueueRepository: IInsertRepository<RankingQueue>, IDeleteRepository<RankingQueue>
    {
        Task<IEnumerable<RankingQueue>> GetByTournamentIdOrderByCreateDate(Guid Id);

    }
}
