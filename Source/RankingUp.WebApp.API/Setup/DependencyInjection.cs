using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RankingUp.Club.Application.Services;
using RankingUp.Club.Data.Repositories;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Configuration;
using RankingUp.Core.Data;
using RankingUp.Player.Application.Services;
using RankingUp.Player.Data.Repositories;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Sport.Application.Services;
using RankingUp.Sport.Data.Repositories;
using RankingUp.Sport.Domain.Repositories;
using RankingUp.Tournament.Application.Events;
using RankingUp.Tournament.Application.Services;
using RankingUp.Tournament.Data.Repositories;
using RankingUp.Tournament.Domain.Repositories;
using System.Reflection;

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
            RegisterServicePlayer(services);
            RegisterServiceTournament(services);
        }


        private static void RegisterConfig(this IServiceCollection services)
        {
            services.AddDapperConfig();
            services.AddAutoMapperConfiguration();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.ToLower().Contains("rankingup")).ToArray();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
            services.AddScoped<IMediatorHandler, MediatorHandler>();
        }
        private static void RegisterCore(this IServiceCollection services)
        {
            services.AddSingleton<DbContext>();
            services.AddSingleton<DbFactory>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
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

        private static void RegisterServicePlayer(this IServiceCollection services)
        {
            services.AddScoped<IPlayerAppService, PlayerAppService>();
            services.AddScoped<IPlayerClubsRepository, PlayerClubsRepository>();
            services.AddScoped<IPlayerSportsRepository, PlayerSportsRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
        }

        private static void RegisterServiceTournament(this IServiceCollection services)
        {
            services.AddScoped<ITournamentsRepository, TournamentsRepository>();
            services.AddScoped<ITournamentTeamRepository, TournamentTeamRepository>();
            services.AddScoped<ITournamentGameRepository, TournamentGameRepository>();
            services.AddScoped<IRankingQueueRepository, RankingQueueRepository>();
            services.AddScoped<IRankingAppService, RankingAppService>();
        }
    }
}
