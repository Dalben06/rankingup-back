using RankingUp.Core.Domain;

namespace RankingUp.Core.Data
{
    public interface IUpdateRepository<T> where T : BaseEntity
    {
        Task<bool> UpdateAsync(T entity);
    }
}
