using RankingUp.Background.Service.Interfaces;
using RankingUp.Background.Service.Service;

namespace RankingUp.WebApp.API.Setup
{
    public static class BackgroundConfig
    {
        public static void RegisterBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<QueueHostedTaskService>();
            services.AddSingleton<IRunEventTaskService, RunEventTaskService>();
            services.AddHostedService<QueueHostedTaskService>();
        }
    }
}
