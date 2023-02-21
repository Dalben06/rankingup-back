using RankingUp.Core.Domain;

namespace RankingUp.Core.Data
{
    public interface IDeleteRepository<T> where T : BaseEntity
    {
        Task<bool> DeleteAsync(T entity);

    }
}
