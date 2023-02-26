using AutoMapper;
using RankingUp.Club.Domain.Entities;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Data;
using RankingUp.Core.Extensions;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Club.Data.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly IBaseRepository _baseRepository;
        private readonly IMapper _mapper;

        public ClubRepository(IBaseRepository baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _baseRepository.SetSql(GetDefaultSql());
            _mapper = mapper;
        }

        private string GetDefaultSql()
        {
            return @"
             SELECT Clubs.*, Sports.*
             FROM Clubs

             LEFT JOIN ClubSports 
             ON ClubSports.ClubId = Clubs.Id

             LEFT JOIN Sports
             ON Sports.Id = ClubSports.SportId

             WHERE 1 = 1
             ";          
        }

        public Task<Domain.Entities.Club> GetById(Guid Id) => this._baseRepository.GetByIdAsync<Domain.Entities.Club,Sports>(Id,SQLMap());
        public Task<Domain.Entities.Club> GetById(int Id) => this._baseRepository.GetByIdAsync<Domain.Entities.Club, Sports>(Id, SQLMap());
        public async Task<Domain.Entities.Club> InsertAsync(Domain.Entities.Club club)
        {
            club = await this._baseRepository.InsertAsync<Domain.Entities.Club>(club);
            if (club.Sports != null && club.Sports.Any())
            {
                var clubSport = _mapper.Map<ClubSport>(club);
                foreach (var sport in club.Sports)
                {
                    var insertItem = clubSport.DeepClone<ClubSport>();
                    insertItem.SportId = sport.Id;
                    await _baseRepository.InsertAsync<ClubSport>(insertItem);
                }
            }
            return club;
        } 
        public Task<bool> UpdateAsync(Domain.Entities.Club entity) => this._baseRepository.UpdateAsync<Domain.Entities.Club>(entity);
        public Task<bool> DeleteAsync(Domain.Entities.Club entity) => this._baseRepository.DeleteAsync<Domain.Entities.Club>(entity);


        private Func<Club.Domain.Entities.Club,Sports, Club.Domain.Entities.Club> SQLMap() 
        {
            var dic = new Dictionary<long, Domain.Entities.Club>();
            return ( Club, Sport) =>
            {
                if (dic.TryGetValue(Club.Id, out Domain.Entities.Club existingClub))
                    Club = existingClub;
                else
                    dic.Add(Club.Id, Club);

                Club.Sports.Add(Sport);
                return Club;
            };

        }

        public Task<IEnumerable<Domain.Entities.Club>> GetClubsBySportId(Guid Id) 
            => _baseRepository.GetAsync<Domain.Entities.Club, Sports>(SQLMap(), " AND Sport.UUId = @Id", new { Id });
    }
}
