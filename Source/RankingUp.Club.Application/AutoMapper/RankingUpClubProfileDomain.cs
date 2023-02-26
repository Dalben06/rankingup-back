using AutoMapper;
using RankingUp.Club.Domain.Entities;

namespace RankingUp.Club.Application.AutoMapper
{
    public class RankingUpClubProfileDomain : Profile
    {
        public RankingUpClubProfileDomain()
        {
            CreateMap<Domain.Entities.Club, ClubSport>()
                .ForMember(dest => dest.ClubId , src => src.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
