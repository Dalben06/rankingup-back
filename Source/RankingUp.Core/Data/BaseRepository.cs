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
            var tableName = typeof(T)?.GetCustomAttribute<TableAttribute>()?.Name;
            if (tableName == null)
                tableName = typeof(T).Name;

            if (tableName.Substring(tableName.Length - 1, 1)[0].ToString().ToLower() != "s")
                tableName += "s";

            return tableName;
        }

        public void Dispose()
        {
            this._context.DbConnection.Close();
            this._context.DbConnection.Dispose();
            GC.SuppressFinalize(this);
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

        public async Task<T> GetByIdAsync<T>(string SQL,long Id) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryFirstAsync<T>(SQL + $" AND {ObterTable<T>()}.Id = @Id", new { Id });
                };
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2>(string SQL, long Id, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T1>(SQL + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3>(string SQL, long Id, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T1>(SQL + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4>(string SQL, long Id, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SQL + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(string SQL, long Id, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SQL + $" AND {ObterTable<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(string SQL, long Id, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SQL + $" AND {ObterTable<T1>()}.Id = @UUId", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }

        public async Task<T> GetByIdAsync<T>(string SQL, Guid UUId) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T>(SQL + $" AND {ObterTable<T>()}.UUId = @UUId", new { UUId }))?.FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2>(string SQL, Guid UUId, Func<T1, T2, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T1>(SQL + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId }))?.FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3>(string SQL, Guid UUId, Func<T1, T2, T3, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T1>(SQL + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4>(string SQL, Guid UUId, Func<T1, T2, T3, T4, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SQL + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5>(string SQL, Guid UUId, Func<T1, T2, T3, T4, T5, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SQL + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<T1> GetByIdAsync<T1, T2, T3, T4, T5, T6>(string SQL, Guid UUId, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SQL + $" AND {ObterTable<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region Get
        public async Task<IEnumerable<T>> GetAsync<T>(string SQL, object param = null) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T>(SQL, param);
                }
            }
            catch (Exception) { throw; }
        }

        public async Task<IEnumerable<T1>> GetAsync<T1, T2>(string SQL, Func<T1, T2, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T1>(SQL , map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3>(string SQL, Func<T1, T2, T3, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T1>(SQL , map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4>(string SQL, Func<T1, T2, T3, T4, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T1>(SQL , map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5>(string SQL, Func<T1, T2, T3, T4, T5, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SQL , map, splitOn: splitOn, param: param);
                }
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<T1>> GetAsync<T1, T2, T3, T4, T5, T6>(string SQL, Func<T1, T2, T3, T4, T5, T6, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    return await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SQL , map, splitOn: splitOn, param: param);
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
