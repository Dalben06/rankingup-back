using RankingUp.Core.Data;
using RankingUp.Core.Domain;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Entities.Filters;

namespace RankingUp.Tournament.Domain.Repositories
{
    public interface ITournamentsRepository : IInsertRepository<Tournaments>, IUpdateRepository<Tournaments>
        , IDeleteRepository<Tournaments>, IGetRepository<Tournaments>
    {
        Task<IEnumerable<Tournaments>> GetAll(bool? isRanking);
        Task<Pagination<Tournaments>> GetTournamentsByFilter(TournamentFilter filter);
        Task<IEnumerable<Tournaments>> GetAllByClub(Guid isRanking);

    }
}
