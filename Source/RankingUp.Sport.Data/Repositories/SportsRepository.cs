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
            _baseRepository.SetSql(GetdefaultSql());
        }

        private string GetdefaultSql()
        {
            return @"
                SELECT sports.* FROM sports WHERE 1 = 1 
            ";
        }

        public Task<IEnumerable<Sports>> GetAll() =>  _baseRepository.GetAsync<Sports>(" AND sports.IsEnable = 1 ");
        public Task<Sports> GetById(Guid Id) => _baseRepository.GetByIdAsync<Sports>(Id);
        public Task<Sports> GetById(int Id) => _baseRepository.GetByIdAsync<Sports>(Id);
    }
}
