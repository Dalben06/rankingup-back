using RankingUp.Club.Application.Services;
using RankingUp.Club.Data.Repositories;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Configuration;
using RankingUp.Core.Data;
using RankingUp.Sport.Application.Services;
using RankingUp.Sport.Data.Repositories;
using RankingUp.Sport.Domain.Repositories;
using System.Configuration;

namespace RankingUp.WebApp.API.Setup
{
    public static class DependencyInjection
    {

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<Settings>(x => configuration.Get<Settings>());

            RegisterConfig(services);
            RegisterCore(services);
            RegisterServiceClub(services);
            RegisterServiceSport(services);
        }


        private static void RegisterConfig(this IServiceCollection services)
        {
            services.AddDapperConfig();
            services.AddAutoMapperConfiguration();
        }
        private static void RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<DbContext>();
            services.AddSingleton<DbFactory>();
            services.AddTransient<IBaseRepository, BaseRepository>();
        }


        private static void RegisterServiceClub(this IServiceCollection services)
        {
            services.AddScoped<IClubAppService, ClubAppService>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IClubSportRepository, ClubSportRepository>();
        }

        private static void RegisterServiceSport(this IServiceCollection services)
        {
            services.AddScoped<ISportAppService, SportAppService>();
            services.AddScoped<ISportsRepository, SportsRepository>();
        }
    }
}
