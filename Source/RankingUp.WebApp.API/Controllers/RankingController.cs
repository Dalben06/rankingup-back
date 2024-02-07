using Microsoft.AspNetCore.Mvc;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ApiController
    {

        private readonly IRankingAppService _rankingAppService;
        private readonly IRankingGameService _rankingGameAppService;
        private readonly IRankingPlayerService _rankingPlayerAppService;

        public RankingController(IRankingAppService rankingAppService,
                                 IRankingGameService rankingGameService,
                                 IRankingPlayerService rankingPlayerService)
        {
            _rankingAppService = rankingAppService;
            _rankingGameAppService = rankingGameService;
            _rankingPlayerAppService = rankingPlayerService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return HttpResponse(await this._rankingAppService.GetAllRankings());
        }

        [HttpPost("GetByFilter")]
        public async Task<IActionResult> GetByFilter( Tournament.Domain.Entities.Filters.TournamentFilter filter)
        {
            return HttpResponse(await this._rankingAppService.GetRankingByFilter(filter));
        }

        [HttpGet("GetRanking/{Id}")]
        public async Task<IActionResult> GetRanking(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.GetRanking(Id));
        }

        [HttpGet("GetPlayers/{Id}")]
        public async Task<IActionResult> GetPlayers(Guid Id)
        {
            return HttpResponse(await this._rankingPlayerAppService.GetPlayers(Id));
        }

        //[HttpGet("GetGame/{Id}")]
        //public async Task<IActionResult> GetMatches(Guid Id)
        //{
        //    return HttpResponse(await this._rankingGameAppService.GetPlayers(Id));
        //}

        [HttpGet("GetGames/{Id}")]
        public async Task<IActionResult> GetMatches(Guid Id, bool? isFinished)
        {
            return HttpResponse(await this._rankingGameAppService.GetGames(Id,isFinished));
        }

        [HttpGet("GetPlayersOnQueue/{Id}")]
        public async Task<IActionResult> GetPlayersOnQueue(Guid Id)
        {
            return HttpResponse(await this._rankingPlayerAppService.GetPlayers(Id));
        }

        [HttpGet("GetGamesGoing/{Id}")]
        public async Task<IActionResult> GetGamesGoing(Guid Id)
        {
            return HttpResponse(await this._rankingGameAppService.GetGames(Id,false));
        }

        [HttpGet("GetRankingsByClub/{Id}")]
        public async Task<IActionResult> GetRankingsByClub(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.GetRankingsByClub(Id));
        }

        [HttpPost("CreateRanking")]
        public async Task<IActionResult> CreateRanking(RankingDetailViewModel model)
        {
            return HttpResponse(await this._rankingAppService.CreateRanking(model));
        }

        [HttpPost("StartRanking/{Id}")]
        public async Task<IActionResult> StartRanking(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.StartRanking(Id,1));
        }

        [HttpPost("FinishRanking/{Id}")]
        public async Task<IActionResult> FinishRanking(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.EndRanking(Id,1));
        }

        [HttpPost("AddPlayer")]
        public async Task<IActionResult> AddPlayer(RankingPlayerViewModel model)
        {
            return HttpResponse(await this._rankingPlayerAppService.AddPlayer(model));
        }

        [HttpPost("AddPlayerQuickly")]
        public async Task<IActionResult> AddPlayerQuickly(RankingAddPlayerQuicklyViewModel model)
        {
            return HttpResponse(await this._rankingPlayerAppService.AddPlayerQuickly(model));
        }

        [HttpPost("CreateGame")]
        public async Task<IActionResult> CreateGame(RankingCreateGameViewModel model)
        {
            return HttpResponse(await this._rankingGameAppService.CreateGame(model));
        }
        [HttpPost("CreateGameUsingQueue/{TornamentId}")]
        public async Task<IActionResult> CreateGameBasedQueue(Guid TornamentId)
        {
            return HttpResponse(await this._rankingGameAppService.CreateGameUsingQueue(TornamentId,1));
        }

        [HttpPut("UpdateGame")]
        public async Task<IActionResult> UpdateGame(RankingGameDetailViewModel model)
        {
            return HttpResponse(await this._rankingGameAppService.UpdateGame(model));
        }


        [HttpPut("UpdateRanking")]
        public async Task<IActionResult> UpdateRanking(RankingDetailViewModel model)
        {
            return HttpResponse(await this._rankingAppService.UpdateRanking(model));
        }


        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> RemoveRanking(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.RemoveRanking(Id, 1));
        }

        [HttpDelete("RemovePlayer/{Id}")]
        public async Task<IActionResult> RemovePlayer(Guid Id)
        {
            return HttpResponse(await this._rankingPlayerAppService.RemovePlayer(Id, 1));
        }

    }
}
