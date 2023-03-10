using Microsoft.AspNetCore.Mvc;
using RankingUp.Club.Application.Services;
using RankingUp.Sport.Application.Services;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    public class SportController : ApiController
    {
        private readonly ISportAppService _sportAppService;

        public SportController(ISportAppService sportAppService)
        {
            this._sportAppService = sportAppService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return HttpResponse(await this._sportAppService.GetAllSports());
        }

        [HttpGet("GetSportById/{Id}")]
        public async Task<IActionResult> GetSportById(Guid Id)
        {
            return HttpResponse(await this._sportAppService.GetSportById(Id));
        }

    }
}
