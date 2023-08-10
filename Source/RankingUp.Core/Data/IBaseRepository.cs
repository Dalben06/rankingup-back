using RankingUp.Core.Domain;

namespace RankingUp.Core.Data
{
    public interface IBaseRepository
    {
        public void OpenConnection();

        Task<T> GetByIdAsync<T>(string SQL,long Id) where T : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2>(string SQL,long Id, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3>(string SQL,long UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4>(string SQL,long UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(string SQL,long UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(string SQL,long UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T> GetByIdAsync<T>(string SQL,Guid UUId) where T : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2>(string SQL,Guid UUId, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3>(string SQL,Guid UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4>(string SQL,Guid UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(string SQL,Guid UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(string SQL,Guid UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity;

        Task<IEnumerable<T>> GetAsync<T>(string SQL, object param = null) where T : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2>(string SQL,Func<T1, T2, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3>(string SQL,Func<T1, T2, T3, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4>(string SQL,Func<T1, T2, T3, T4, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5>(string SQL,Func<T1, T2, T3, T4, T5, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5, T6>(string SQL,Func<T1, T2, T3, T4, T5, T6, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;

        Task<Pagination<T>> GetPagination<T>(Filter filter, string SQL, object param = null) where T : BaseEntity;
        Task<Pagination<T1>> GetPagination<T1, T2>(Filter filter, string SQL, Func<T1, T2, T1> map,object param = null, string splitOn = "Id") where T1 : BaseEntity;
        //Task<IEnumerable<T1>> GetPagination<T1, T2, T3>(string SQL, Func<T1, T2, T3, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        //Task<IEnumerable<T1>> GetPagination<T1, T2, T3, T4>(string SQL, Func<T1, T2, T3, T4, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        //Task<IEnumerable<T1>> GetPagination<T1, T2, T3, T4, T5>(string SQL, Func<T1, T2, T3, T4, T5, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        //Task<IEnumerable<T1>> GetPagination<T1, T2, T3, T4, T5, T6>(string SQL, Func<T1, T2, T3, T4, T5, T6, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity;

        Task<T> InsertAsync<T>(T entity) where T : BaseEntity;
        Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
        Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    }
}
