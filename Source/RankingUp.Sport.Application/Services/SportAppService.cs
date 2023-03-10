using AutoMapper;
using RankingUp.Core.Domain;
using RankingUp.Sport.Application.ViewModels;
using RankingUp.Sport.Domain.Repositories;

namespace RankingUp.Sport.Application.Services
{
    public class SportAppService : ISportAppService
    {

        private readonly ISportsRepository _sportsRepository;
        private readonly IMapper _mapper;

        public SportAppService(ISportsRepository sportsRepository, IMapper mapper)
        {
            _sportsRepository = sportsRepository;
            _mapper = mapper;
        }

        public async Task<RequestResponse<IEnumerable<SportViewModel>>> GetAllSports()
        {
            try
            {
                return new RequestResponse<IEnumerable<SportViewModel>>(
                    this._mapper.Map<IEnumerable<SportViewModel>>(await _sportsRepository.GetAll())
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<SportViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<SportViewModel>> GetSportById(Guid Id)
        {
            try
            {
                return new RequestResponse<SportViewModel>(
                    this._mapper.Map<SportViewModel>(await _sportsRepository.GetById(Id))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<SportViewModel>(ex.Message);
            }
        }
    }
}
