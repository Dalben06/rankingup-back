using RankingUp.Core.Domain;
using RankingUp.Player.Application.ViewModel;

namespace RankingUp.Player.Application.Services
{
    public interface IPlayerAppService
    {
        Task<RequestResponse<IEnumerable<PlayerViewModel>>> GetPlayers();
        Task<RequestResponse<IEnumerable<PlayerViewModel>>> GetPlayersFromClubId(Guid Id);
        Task<RequestResponse<PlayerViewModel>> GetPlayer(Guid Id);
        Task<RequestResponse<PlayerViewModel>> CreatePlayer(PlayerCreateViewModel model);
        Task<RequestResponse<PlayerViewModel>> UpdatePlayer(PlayerViewModel model);
        Task<RequestResponse<PlayerSportViewModel>> AddSport(PlayerSportViewModel model);
        Task<NoContentResponse> RemoveSport(Guid Id, int UserId);
        Task<RequestResponse<PlayerClubViewModel>> AddClub(PlayerClubViewModel model);
        Task<NoContentResponse> RemoveClub(Guid Id, int UserId);

    }
}
