using AutoMapper;
using RankingUp.Sport.Application.ViewModels;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Sport.Application.AutoMapper
{
    public class RankingUpSportProfileDomain : Profile
    {

        public RankingUpSportProfileDomain()
        {
            CreateMap<SportViewModel, Sports>().ReverseMap();
        }
    }
}
