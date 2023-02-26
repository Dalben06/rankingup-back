using RankingUp.Club.Application.ViewModels;
using RankingUp.Core.Domain;

namespace RankingUp.Club.Application.Services
{
    public interface IClubAppService
    {
        Task<RequestResponse<IEnumerable<ClubViewModel>>> GetClubsFromSports(Guid Id);
        Task<RequestResponse<ClubDetailViewModel>> GetClubById(Guid Id);
        Task<RequestResponse<ClubDetailViewModel>> CreateClub(ClubDetailViewModel clubDetailViewModel);
        Task<RequestResponse<ClubDetailViewModel>> UpdateClub(ClubDetailViewModel clubDetailViewModel);
        Task<NoContentResponse> DisableClub(Guid Id, int UserId);
        Task<RequestResponse<ClubSportViewModel>> AddSportOnClub(ClubSportViewModel clubDetailViewModel);
        Task<NoContentResponse> RemoveSportOnClub(Guid Id, int UserId);


    }
}
