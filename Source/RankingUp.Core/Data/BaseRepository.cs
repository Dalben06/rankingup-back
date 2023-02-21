using Dapper;
using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Core.Extensions;
using System.Data;
using System.Reflection;
using static Dapper.SqlMapper;

namespace RankingUp.Core.Data
{
    public class BaseRepository : IBaseRepository, IDisposable
    {
        private readonly DbContext _context;
        public string SqlBase;
        public BaseRepository(DbContext context)
        {
            this._context = context;
        }

        public void OpenConnection()
        {
            if (this._context.DbConnection.State == ConnectionState.Closed || this._context.DbConnection.State == ConnectionState.Broken)
                this._context.DbConnection.Open();
        }
        private string ObterTable<T>()
        {
            return typeof(T)?.GetCustomAttribute<TableAttribute>()?.Name ?? "";
        }

        public void Dispose()
        {
            this._context.DbConnection.Close();
            this._context.DbConnection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SetSql(string SQL)
        {
            this.SqlBase = SQL;
        }

        //private string GetPaginationSQL<T>(Paginacao<T> pag, string sql)
        //{
        //    sql += @$"
        //        LIMIT {pag.LinhasPorPagina}
        //        OFFSET {pag.ObterOFFSET()}
        //    ";
        //    return sql;
        //}

        //private async Task<int> SelectCountPaginacaoAsync(string sql)
        //{
        //    var spliSQL = sql.Split("FROM");

        //    var sqlCount = @$"
        //        SELECT COUNT(*) FROM {spliSQL.LastOrDefault()}
        //    ";
        //    using (var con = this._context.NewConnection)
        //    {
        //        return await con.QueryFirstAsync<int>(sqlCount);
        //    }
        //}

        #region GetById

        public async Task<T> GetByIdAsync<T>(long Id) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryFirstAsync<T>(SqlBase + $" AND {ObterTable<T>()}.Id = @Id", new { Id, Deletado = false });
                };
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2>(long Id, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T1>(SqlBase + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3>(long Id, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T1>(SqlBase + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4>(long Id, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SqlBase + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(long Id, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SqlBase + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(long Id, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SqlBase + $" AND {ObterTable<T1>()}.Id = @UUId", map, splitOn: splitOn, param: new { Id, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }

        public async Task<T> GetByIdAsync<T>(Guid UUId) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryFirstAsync<T>(SqlBase + $" AND {ObterTable<T>()}.UUId = @UUId", new { UUId, Deletado = false });
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2>(Guid UUId, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T1>(SqlBase + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3>(Guid UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T1>(SqlBase + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4>(Guid UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SqlBase + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(Guid UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SqlBase + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(Guid UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SqlBase + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId, Deletado = false })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region Get
        public async Task<IEnumerable<T>> GetAsync<T>(object param = null) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T>(SqlBase, param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2>(Func<T1, T2, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T1>(SqlBase, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string addicionalConditionaisSQL = null, object param = null) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T>(SqlBase + addicionalConditionaisSQL, param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2>(Func<T1, T2, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T1>(SqlBase + addicionalConditionaisSQL, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3>(Func<T1, T2, T3, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T1>(SqlBase + addicionalConditionaisSQL, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T1>(SqlBase + addicionalConditionaisSQL, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SqlBase + addicionalConditionaisSQL, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T1> map, string addicionalConditionaisSQL = null, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SqlBase + addicionalConditionaisSQL, map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }


        #endregion

        #region Action
        public Task<T> InsertAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                return this._context.DbConnection.CoreInsertAsync<T>(entity);
            }
            catch (Exception) { throw; }
        }

        public Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                return this._context.DbConnection.CoreUpdateAsync<T>(entity);
            }
            catch (Exception) { throw; }
        }

        public Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                return this._context.DbConnection.CoreUpdateAsync<T>(entity);
            }
            catch (Exception) { throw; }
        }
        #endregion
    }
}
