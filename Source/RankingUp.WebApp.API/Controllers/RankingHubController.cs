using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RankingUp.Tournament.Application.Hubs;
using RankingUp.Tournament.Domain.Events;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[Controller]")]
    public class RankingHubController : ApiController
    {

        private readonly IHubContext<RankingHub> _hubContext;
        public RankingHubController(IHubContext<RankingHub> hubContext  ) { _hubContext = hubContext; }


        [HttpPost("Notify/{Id}")]
        public async Task<IActionResult> Notify(Guid Id,[FromBody] RankingUpdateSignalr model)
        {
            await this._hubContext.Clients.Groups(Id.ToString().ToLower()).SendAsync("rankingUpdate", model);
            return NoContent();
        }

    }
}
