using AutoMapper;
using RankingUp.Player.Application.ViewModel;
using RankingUp.Player.Domain.Entities;

namespace RankingUp.Player.Application.AutoMapper
{
    public class RankingUpPlayerProfile : Profile
    {

        public RankingUpPlayerProfile()
        {
            CreateMap<PlayerCreateViewModel,Players>()
                .ConstructUsing(p => new Players(p.UserId,p.Name,p.Description))
                .ForMember(dest => dest.UUId, src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
                .ReverseMap();
            CreateMap<Players, PlayerViewModel>().ReverseMap();
            CreateMap<PlayerSports, PlayerSportViewModel>().ReverseMap();
            CreateMap<PlayerClubs, PlayerClubViewModel>().ReverseMap();
        }
    }
}
