using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Data;

namespace RankingUp.Club.Data.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly IBaseRepository _baseRepository;

        public ClubRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }
        public Task<Domain.Entities.Club> GetById(Guid Id) => this._baseRepository.GetByIdAsync<Domain.Entities.Club>(Id);
        public Task<Domain.Entities.Club> GetById(int Id) => this._baseRepository.GetByIdAsync<Domain.Entities.Club>(Id);
        public Task<Domain.Entities.Club> InsertAsync(Domain.Entities.Club entity) => this._baseRepository.InsertAsync<Domain.Entities.Club>(entity);
        public Task<bool> UpdateAsync(Domain.Entities.Club entity) => this._baseRepository.UpdateAsync<Domain.Entities.Club>(entity);
        public Task<bool> DeleteAsync(Domain.Entities.Club entity) => this._baseRepository.DeleteAsync<Domain.Entities.Club>(entity);

    }
}
