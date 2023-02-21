using RankingUp.Core.Domain;

namespace RankingUp.Core.Data
{
    public interface IInsertRepository<T> where T : BaseEntity
    {
        Task<T> InsertAsync(T entity);

    }
}
