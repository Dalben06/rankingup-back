using Microsoft.AspNetCore.Mvc;
using RankingUp.Tournament.Application.Services;
using RankingUp.Tournament.Application.ViewModels;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ApiController
    {

        private readonly IRankingAppService _rankingAppService;

        public RankingController(IRankingAppService rankingAppService)
        {
            _rankingAppService = rankingAppService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return HttpResponse(await this._rankingAppService.GetAllRankings());
        }

        [HttpGet("GetRanking/{Id}")]
        public async Task<IActionResult> GetRanking(Guid Id)
        {
            return HttpResponse(await this._rankingAppService.GetRanking(Id));
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

    }
}
