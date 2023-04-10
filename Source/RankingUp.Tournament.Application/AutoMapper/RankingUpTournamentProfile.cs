using AutoMapper;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities;

namespace RankingUp.Tournament.Application.AutoMapper
{
    public class RankingUpTournamentProfile : Profile
    {

        public RankingUpTournamentProfile()
        {
            CreateMap<RankingDetailViewModel, Tournaments>()
               .ConstructUsing(c => new Tournaments(c.Name,c.Description,c.Address,c.AddressNumber,c.AddressComplement,c.AddressDistrict,
               c.City,c.State,c.Country,c.PostalCode,c.Phone,c.Email,c.EventDate,c.EventHourStart,c.EventHourEnd,true,true,c.OnlyClubMembers,
               c.Price,c.MemberPrice,c.MatchSameTime,null,c.UserId,c.Latitude,c.Longitude,c.AutoQueue,c.HasNotificationToPlayer))
               .ForMember(dest => dest.UUId, src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
               .ReverseMap();

            CreateMap<RankingPlayerViewModel, TournamentTeam>()
              .ForMember(dest => dest.UUId, src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
              .ReverseMap();


            CreateMap<RankingGameDetailViewModel, TournamentGame>()
             .ForMember(dest => dest.UUId, src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
             .ReverseMap();

            CreateMap<RankingQueue,RankingPlayerQueueViewModel > ()
               .ForMember(dest => dest.Player, src => src.MapFrom(src => src.Team.Player.Name ?? ""))
              .ReverseMap();

        }
    }
}
