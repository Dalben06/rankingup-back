using RankingUp.Core.Data;
using RankingUp.Sport.Domain.Entities;
using RankingUp.Sport.Domain.Repositories;

namespace RankingUp.Sport.Data.Repositories
{
    public class SportsRepository : ISportsRepository
    {
        private readonly IBaseRepository _baseRepository;

        public SportsRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @"
                SELECT sports.* FROM sports WHERE 1 = 1 
            ";
        }

        public Task<IEnumerable<Sports>> GetAll() =>  _baseRepository.GetAsync<Sports>(GetDefaultSql());
        public Task<IEnumerable<Sports>> GetByIds(Guid[] Id) => _baseRepository.GetAsync<Sports>(GetDefaultSql() + " AND  sports.Id in @Id", new {Id});
        public Task<Sports> GetById(Guid Id) => _baseRepository.GetByIdAsync<Sports>(GetDefaultSql(), Id);
        public Task<Sports> GetById(int Id) => _baseRepository.GetByIdAsync<Sports>(GetDefaultSql(), Id);

       
    }
}
