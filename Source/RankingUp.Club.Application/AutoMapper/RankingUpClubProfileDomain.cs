using AutoMapper;
using RankingUp.Club.Application.ViewModels;
using RankingUp.Club.Domain.Entities;

namespace RankingUp.Club.Application.AutoMapper
{
    public class RankingUpClubProfileDomain : Profile
    {
        public RankingUpClubProfileDomain()
        {
            CreateMap<Clubs, ClubSport>()
                .ForMember(dest => dest.ClubId , src => src.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<ClubDetailViewModel, Clubs>()
                .ConstructUsing(c => new Clubs(
                    c.Name, c.Description, c.Address, c.City, c.State, c.Country, c.Phone, c.PostalCode, c.BusinessHourStart
                    , c.BusinessHourEnd, c.FacebookUrl, c.InstagramUrl, c.TwitterUrl, null, true, c.Email,c.AddressNumber,c.AddressComplement,c.AddressDistrict, c.UserId
                    ))
                .ForMember(dest => dest.UUId, src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
                .ReverseMap();


            CreateMap<ClubViewModel, Clubs>().ReverseMap();
            CreateMap<ClubSportViewModel, ClubSport>().ReverseMap();
        }
    }
}
