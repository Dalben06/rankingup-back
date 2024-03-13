using Dapper;
using Dapper.Contrib.Extensions;
using MySqlX.XDevAPI.Common;
using RankingUp.Core.Domain;
using RankingUp.Core.Extensions;
using RankingUp.Core.Helpers;
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

        private void OpenConnection(IDbConnection con)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
        }
        private string GetTableName<T>()
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


        private string GetPaginationSQL<T>(Pagination<T> pag, string sql)
        {
            // (CURRENT PAGE - 1) * NUMBER OF RECORDS PER PAGE;
            sql += @$"
                LIMIT {pag.ItensPerPage}
                OFFSET {((pag.CurrentPage - 1) * pag.ItensPerPage)} 
            ";
            return sql;
        }

        private async Task<int> SelectCountPaginacaoAsync<T>(string sql, object param)
        {
            var spliSQL = sql.Split("FROM");

            var sqlCount = @$"
                SELECT COUNT( DISTINCT {GetTableName<T>()}.{DapperHelper.GetPropertyKeyName<T>()}) FROM {spliSQL.LastOrDefault()}
            ";
            using (var con = this._context.NewConnection)
            {
                this.OpenConnection(con);
                return await con.QueryFirstAsync<int>(sqlCount, param);
            }
        }

        #region GetById

        public async Task<T> GetByIdAsync<T>(string SQL,long Id) where T : BaseEntity
        {
            try
            {
                using (var con = this._context.NewConnection)
                {
                    this.OpenConnection(con);
                    return await con.QueryFirstAsync<T>(SQL + $" AND {GetTableName<T>()}.Id = @Id", new { Id });
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T1>(SQL + $" AND {GetTableName<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T1>(SQL + $" AND {GetTableName<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SQL + $" AND {GetTableName<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SQL + $" AND {GetTableName<T1>()}.Id = @Id", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SQL + $" AND {GetTableName<T1>()}.Id = @UUId", map, splitOn: splitOn, param: new { Id })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T>(SQL + $" AND {GetTableName<T>()}.UUId = @UUId", new { UUId }))?.FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T1>(SQL + $" AND {GetTableName<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId }))?.FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T1>(SQL + $" AND {GetTableName<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T1>(SQL + $" AND {GetTableName<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T1>(SQL + $" AND {GetTableName<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
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
                    this.OpenConnection(con);
                    return (await con.QueryAsync<T1, T2, T3, T4, T5, T6, T1>(SQL + $" AND {GetTableName<T1>()}.UUId = @UUId", map, splitOn: splitOn, param: new { UUId })).FirstOrDefault();
                }
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region GetFirstOrDefault

        //public Task<T1> GetFirstOrDefaultAsync<T1, T2, T3>(string SQL, Func<T1, T2, T3, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        //{
        //    try
        //    {
        //        using (var con = this._context.NewConnection)
        //        {
        //            return await con.QueryFirstOrDefault<T1>();
        //        };
        //    }
        //    catch (Exception) { throw; }
        //}

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


        #region Pagination

        public async Task<Pagination<T>> GetPagination<T>(Filter filter,string sql, object param = null) where T : BaseEntity
        {
            var result = new Pagination<T>();

            result.TotalItens = await SelectCountPaginacaoAsync<T>(sql, param);

            if (!filter.Ignore)
            {
                result.CurrentPage = filter.CurrentPage;
                result.ItensPerPage = filter.ItensPerPage;
            }
            else
            {
                result.ItensPerPage = result.TotalItens;
                result.CurrentPage = 1;
            }

            if (result.TotalItens > 0)
            {
                using (var con = this._context.NewConnection)
                {
                    this.OpenConnection(con);
                    result.Datasource = (await con.QueryAsync<T>(GetPaginationSQL<T>(result,sql),param))?.ToList();
                }
            }
            return result;

        }

        public async Task<Pagination<T1>> GetPagination<T1, T2>(Filter filter, string sql, Func<T1, T2, T1> map, object param = null, string splitOn = "Id") where T1 : BaseEntity
        {
            var result = new Pagination<T1>();

            result.TotalItens = await SelectCountPaginacaoAsync<T1>(sql, param);

            if (!filter.Ignore)
            {
                result.CurrentPage = filter.CurrentPage;
                result.ItensPerPage = filter.ItensPerPage;
            }
            else
            {
                result.ItensPerPage = result.TotalItens;
                result.CurrentPage = 1;
            }

            if (result.TotalItens > 0)
            {
                using (var con = this._context.NewConnection)
                {
                    this.OpenConnection(con);
                    result.Datasource = (await con.QueryAsync<T1,T2,T1>(GetPaginationSQL<T1>(result, sql), map, param))?.ToList();
                }
            }
            return result;

        }

        
        #endregion
    }
}
