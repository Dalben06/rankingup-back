using Microsoft.AspNetCore.SignalR;

namespace RankingUp.Tournament.Application.Hubs
{
    public class RankingHub : Hub
    {
        public async Task JoinRanking(string rankingid)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, rankingid.ToLower());
            await Clients.Caller.SendAsync("joinned", "OK");
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
