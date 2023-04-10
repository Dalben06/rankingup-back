using RankingUp.Tournament.Application.Hubs;

namespace RankingUp.WebApp.API.Setup
{
    public static class HubConfig
    {

        public static void MapHubs(this WebApplication app)
        {
            app.MapHub<RankingHub>("/api/rankinghub");
        }

    }
}
