using RankingUp.Club.Application.AutoMapper;
using RankingUp.Sport.Application.AutoMapper;

namespace RankingUp.WebApp.API.Setup
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(RankingUpClubProfileDomain));
            services.AddAutoMapper(typeof(RankingUpClubProfileViewModelDomain));
            services.AddAutoMapper(typeof(RankingUpSportProfileDomain));
        }
    }
}
