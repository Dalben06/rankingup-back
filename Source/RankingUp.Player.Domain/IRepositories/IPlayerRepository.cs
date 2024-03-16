using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;

namespace RankingUp.Player.Domain.IRepositories
{
    public interface IPlayerRepository : IInsertRepository<Players>, IUpdateRepository<Players>
        , IDeleteRepository<Players>, IGetRepository<Players>
    {
        Task<IEnumerable<Players>> GetAll();

        Task<Players> GetByPhoneNumber(string phoneNumber);
    }
}
