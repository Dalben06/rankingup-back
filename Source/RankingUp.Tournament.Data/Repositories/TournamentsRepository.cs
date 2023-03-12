using RankingUp.Core.Data;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;

namespace RankingUp.Tournament.Data.Repositories
{
    public class TournamentsRepository : ITournamentsRepository
    {
        private readonly IBaseRepository _baseRepository;

        public TournamentsRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @"
             SELECT Tournaments.*
             FROM Tournaments

             WHERE 1 = 1
             Tournaments.IsDeleted = 0 
             ";
        }

        public Task<IEnumerable<Tournaments>> GetAll() => _baseRepository.GetAsync<Tournaments>(GetDefaultSql());
        public Task<Tournaments> GetById(Guid Id) => _baseRepository.GetByIdAsync<Tournaments>(GetDefaultSql(), Id);
        public Task<Tournaments> GetById(int Id) => _baseRepository.GetByIdAsync<Tournaments>(GetDefaultSql(), Id);
        public Task<Tournaments> InsertAsync(Tournaments entity) => _baseRepository.InsertAsync<Tournaments>(entity);
        public Task<bool> UpdateAsync(Tournaments entity) => _baseRepository.UpdateAsync<Tournaments>(entity);
        public Task<bool> DeleteAsync(Tournaments entity) => _baseRepository.DeleteAsync<Tournaments>(entity);


    }
}
