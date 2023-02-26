using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RankingUp.Club.Application.Services;
using RankingUp.Club.Application.ViewModels;

namespace RankingUp.WebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ApiController
    {
        private readonly IClubAppService _clubAppService;

        public ClubController(IClubAppService clubAppService)
        {
            this._clubAppService= clubAppService;
        }

        [HttpGet("GetClubsBySportId/{Id}")]
        public async Task<IActionResult> GetClubsBySportId(Guid Id)
        {
            return HttpResponse(await this._clubAppService.GetClubsFromSports(Id));
        }

        [HttpGet("GetClubById/{Id}")]
        public async Task<IActionResult> GetClubById(Guid Id)
        {
            return HttpResponse(await this._clubAppService.GetClubById(Id));
        }


        [HttpPost("CreateClub")]
        public async Task<IActionResult> CreateClub(ClubDetailViewModel model)
        {
            return HttpResponse(await this._clubAppService.CreateClub(model));
        }

        [HttpPost("AddSportOnClub")]
        public async Task<IActionResult> AddSportOnClub(ClubSportViewModel model)
        {
            return HttpResponse(await this._clubAppService.AddSportOnClub(model));
        }


        [HttpPut("UpdateClub")]
        public async Task<IActionResult> UpdateClub(ClubDetailViewModel model)
        {
            return HttpResponse(await this._clubAppService.UpdateClub(model));
        }

        [HttpDelete("DisableClub")]
        public async Task<IActionResult> DisableClub(Guid Id)
        {
            return HttpResponse(await this._clubAppService.DisableClub(Id,1));
        }

        [HttpDelete("RemoveSportOnClub")]
        public async Task<IActionResult> RemoveSportOnClub(Guid Id)
        {
            return HttpResponse(await this._clubAppService.RemoveSportOnClub(Id, 1));
        }

    }
}
