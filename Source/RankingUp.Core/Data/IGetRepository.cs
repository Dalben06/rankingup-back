using RankingUp.Core.Domain;
using System.Threading.Tasks;

namespace RankingUp.Core.Data
{
    public interface IGetRepository<T> where T: BaseEntity 
    {
        Task<T> GetById(Guid Id);
        Task<T> GetById(int Id);
    }
}
