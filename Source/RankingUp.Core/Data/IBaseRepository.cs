using RankingUp.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingUp.Core.Data
{
    public interface IBaseRepository
    {
        public void SetSql(string SQL);
        public void OpenConnection();

        Task<T> GetByIdAsync<T>(long Id) where T : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2>(long Id, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3>(long UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4>(long UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(long UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(long UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T> GetByIdAsync<T>(Guid UUId) where T : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2>(Guid UUId, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3>(Guid UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4>(Guid UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(Guid UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity;
        Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(Guid UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity;

        Task<IEnumerable<T>> GetAsync<T>(string addicionalConditionaisSQL = null, object param = null) where T : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2>(Func<T1, T2, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3>(Func<T1, T2, T3, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity;
        Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity;


        Task<T> InsertAsync<T>(T entity) where T : BaseEntity;
        Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
        Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    }
}
