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

        public ClubRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        private string GetDefaultSql()
        {
            return @"
             SELECT Clubs.*, Sports.*
             FROM Clubs

             LEFT JOIN ClubSports 
             ON ClubSports.ClubId = Clubs.Id
             AND ClubSports.IsDeleted = FALSE

             LEFT JOIN Sports
             ON Sports.Id = ClubSports.SportId

             WHERE 1 = 1
             AND Clubs.IsDeleted = FALSE
             ";          
        }

        public Task<Clubs> GetById(Guid Id) => this._baseRepository.GetByIdAsync<Clubs,Sports>(GetDefaultSql(),Id,SQLMap());
        public Task<Clubs> GetById(int Id) => this._baseRepository.GetByIdAsync<Clubs, Sports>(GetDefaultSql(), Id, SQLMap());
        public Task<IEnumerable<Clubs>> GetClubs(Guid[] Id) => this._baseRepository.GetAsync<Clubs, Sports>(GetDefaultSql() + " AND Clubs.UUId = ANY(@Id)", SQLMap(), new {Id});
        public Task<IEnumerable<Clubs>> GetAll() => this._baseRepository.GetAsync<Clubs, Sports>(GetDefaultSql(), SQLMap());
        public Task<Clubs> InsertAsync(Clubs club) => this._baseRepository.InsertAsync<Clubs>(club);
        public Task<bool> UpdateAsync(Clubs entity) => this._baseRepository.UpdateAsync<Clubs>(entity);
        public Task<bool> DeleteAsync(Clubs entity) => this._baseRepository.DeleteAsync<Clubs>(entity);
        public Task<IEnumerable<Clubs>> GetClubsBySportId(Guid Id)
           => _baseRepository.GetAsync<Clubs, Sports>(GetDefaultSql() + " AND Sports.UUId = @Id", SQLMap(), new { Id });

        public Task<IEnumerable<Clubs>> GetClubAndSportId(Guid ClubId, Guid SportId)
        {
            var SQL = @"
                 SELECT Clubs.*, Sports.*
                 FROM Clubs

                 INNER JOIN ClubSports 
                 ON ClubSports.ClubId = Clubs.Id
                 AND ClubSports.IsDeleted = FALSE

                 INNER JOIN Sports
                 ON Sports.Id = ClubSports.SportId

                 WHERE 1 = 1
                 AND Clubs.IsDeleted = FALSE
                 AND Sports.UUId = @SportId 
                  AND Clubs.UUId = @ClubId
                ";
            return _baseRepository.GetAsync<Clubs, Sports>(SQL, SQLMap(), new { ClubId, SportId });
        } 

        private Func<Clubs,Sports, Clubs> SQLMap() 
        {
            var dic = new Dictionary<long, Clubs>();
            return ( Club, Sport) =>
            {
                if (dic.TryGetValue(Club.Id, out Clubs existingClub))
                    Club = existingClub;
                else
                    dic.Add(Club.Id, Club);

                if (Club.Sports == null)
                    Club.Sports = new List<Sports>();   

                if (Sport != null && !Club.Sports.Any(x => x.Id == Sport.Id))
                    Club.Sports.Add(Sport);
                return Club;
            };

        }

        
    }
}
