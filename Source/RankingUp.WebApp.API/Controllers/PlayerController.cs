using Microsoft.AspNetCore.Mvc;
using RankingUp.Player.Application.Services;
using RankingUp.Player.Application.ViewModel;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ApiController
    {
        private readonly IPlayerAppService _playerAppService;

        public PlayerController(IPlayerAppService playerAppService)
        {
            _playerAppService = playerAppService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return HttpResponse(await this._playerAppService.GetPlayers());
        }

        [HttpGet("GetPlayer/{Id}")]
        public async Task<IActionResult> GetPlayer(Guid Id)
        {
            return HttpResponse(await this._playerAppService.GetPlayer(Id));
        }

        [HttpGet("GetPlayersFromClub/{Id}")]
        public async Task<IActionResult> GetPlayerById(Guid Id)
        {
            return HttpResponse(await this._playerAppService.GetPlayersFromClubId(Id));
        }


        [HttpPost("CreatePlayer")]
        public async Task<IActionResult> CreatePlayer(PlayerCreateViewModel model)
        {
            return HttpResponse(await this._playerAppService.CreatePlayer(model));
        }

        [HttpPost("AddSport")]
        public async Task<IActionResult> AddSport(PlayerSportViewModel model)
        {
            return HttpResponse(await this._playerAppService.AddSport(model));
        }

        [HttpPost("AddClub")]
        public async Task<IActionResult> AddClub(PlayerClubViewModel model)
        {
            return HttpResponse(await this._playerAppService.AddClub(model));
        }


        [HttpPut("UpdatePlayer")]
        public async Task<IActionResult> UpdatePlayer(PlayerViewModel model)
        {
            return HttpResponse(await this._playerAppService.UpdatePlayer(model));
        }


        [HttpDelete("RemoveSport/{Id}")]
        public async Task<IActionResult> RemoveSport(Guid Id)
        {
            return HttpResponse(await this._playerAppService.RemoveSport(Id, 1));
        }

        [HttpDelete("RemoveClub/{Id}")]
        public async Task<IActionResult> RemoveClub(Guid Id)
        {
            return HttpResponse(await this._playerAppService.RemoveClub(Id, 1));
        }
    }
}
