using AutoMapper;
using RankingUp.Club.Application.ViewModels;

namespace RankingUp.Club.Application.AutoMapper
{
    public class RankingUpClubProfileViewModelDomain : Profile
    {

        public RankingUpClubProfileViewModelDomain()
        {
            CreateMap<ClubViewModel, Domain.Entities.Club>().ReverseMap();
            CreateMap<ClubDetailViewModel, Domain.Entities.Club>()
                .ConstructUsing(c => new Domain.Entities.Club(
                    c.Name, c.Description, c.Address,c.City,c.State,c.Country,c.Phone,c.PostalCode,c.BusinessHourStart
                    ,c.BusinessHourEnd,c.FacebookUrl,c.InstagramUrl,c.TwitterUrl,null,true,c.Email,c.UserId
                    ))
                .ForMember(dest => dest.UUId , src => src.MapFrom(src => src.UUId == Guid.Empty ? Guid.NewGuid() : src.UUId))
                .ReverseMap();
        }

    }
}
