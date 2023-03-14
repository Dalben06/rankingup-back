using RankingUp.Club.Application.AutoMapper;
using RankingUp.Player.Application.AutoMapper;
using RankingUp.Sport.Application.AutoMapper;
using RankingUp.Tournament.Application.AutoMapper;

namespace RankingUp.WebApp.API.Setup
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(RankingUpClubProfileDomain));
            services.AddAutoMapper(typeof(RankingUpSportProfileDomain));
            services.AddAutoMapper(typeof(RankingUpPlayerProfile));
            services.AddAutoMapper(typeof(RankingUpTournamentProfile));
        }
    }
}
