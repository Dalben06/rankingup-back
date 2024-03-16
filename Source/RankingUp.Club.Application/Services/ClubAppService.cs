using AutoMapper;
using RankingUp.Club.Application.ViewModels;
using RankingUp.Club.Domain.Entities;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Domain;
using RankingUp.Sport.Application.ViewModels;
using RankingUp.Sport.Domain.Entities;
using RankingUp.Sport.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Club.Application.Services
{
    public class ClubAppService : IClubAppService
    {

        private readonly IClubRepository _clubRepository;
        private readonly IClubSportRepository _clubSportRepository;
        private readonly ISportsRepository _sportsRepository;
        private readonly IMapper _mapper;
        public ClubAppService(IClubRepository clubRepository
                                , IMapper mapper
                                , IClubSportRepository clubSportRepository
                                , ISportsRepository sportsRepository)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
            _clubSportRepository = clubSportRepository;
            _sportsRepository = sportsRepository;
        }

        public async Task<RequestResponse<InicialDataViewModel>> GetInicialData()
        {
            try
            {
                var result = new InicialDataViewModel
                {
                    Sports = _mapper.Map<IEnumerable<SportViewModel>>(await _sportsRepository.GetAll())
                };
                return new RequestResponse<InicialDataViewModel>(result, new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<InicialDataViewModel>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<ClubViewModel>>> GetAll()
        {
            try
            {
                return new RequestResponse<IEnumerable<ClubViewModel>>(this._mapper.Map<IEnumerable<ClubViewModel>>(await _clubRepository.GetAll()), new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<ClubViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<ClubDetailViewModel>> GetClubById(Guid Id)
        {
            try
            {
                return new RequestResponse<ClubDetailViewModel>(
                    this._mapper.Map<ClubDetailViewModel>(await _clubRepository.GetById(Id))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<ClubDetailViewModel>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<ClubViewModel>>> GetClubsFromSports(Guid Id)
        {
            try
            {
                return new RequestResponse<IEnumerable<ClubViewModel>>(this._mapper.Map<IEnumerable<ClubViewModel>>(await _clubRepository.GetClubsBySportId(Id)), new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<ClubViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<ClubDetailViewModel>> CreateClub(CreateClubViewModel clubDetailViewModel)
        {
            var noticable = new Notifiable();
            try
            {
                var club = _mapper.Map<Clubs>(clubDetailViewModel);
                noticable.AddNotifications(club.Notifications);

                IEnumerable<Sports> sports = Enumerable.Empty<Sports>();
                if (clubDetailViewModel.SportsUUId.Any())
                    sports = await _sportsRepository.GetByIds(clubDetailViewModel.SportsUUId.ToArray());

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        club = await _clubRepository.InsertAsync(club);
                        if (sports.Any())
                        {
                            foreach (var sport in sports)
                                await _clubSportRepository.InsertAsync(new ClubSport(club.Id,sport.Id, club.CreatePersonId));
                        }
                        scope.Complete();
                    }
                    return new RequestResponse<ClubDetailViewModel>(_mapper.Map<ClubDetailViewModel>(await _clubRepository.GetById(club.UUId)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<ClubDetailViewModel>(noticable);
        }

        public async Task<RequestResponse<ClubDetailViewModel>> UpdateClub(ClubDetailViewModel clubDetailViewModel)
        {
            var noticable = new Notifiable();
            try
            {
                var club = _mapper.Map<Clubs>(clubDetailViewModel);
                var orig = await _clubRepository.GetById(club.UUId);

                if (orig is null)
                    noticable.AddNotification("Clube não encontrado!");

                club.Validate();
                noticable.AddNotifications(club.Notifications);

                if (noticable.Valid)
                {
                    club.Id = orig.Id;
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _clubRepository.UpdateAsync(club);
                        scope.Complete();
                    }
                    return new RequestResponse<ClubDetailViewModel>(_mapper.Map<ClubDetailViewModel>(await _clubRepository.GetById(club.UUId)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<ClubDetailViewModel>(noticable);
        }

        public async Task<NoContentResponse> DisableClub(Guid Id, int UserId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _clubRepository.GetById(Id);

                if (orig is null)
                   throw new Exception("Clube não encontrado!");

                orig.Disable(UserId);

                if (noticable.Valid)
                {
                    var sports = (await this._clubSportRepository.GetSportFromClubId(Id)).Select(x => { x.Disable(UserId); return x; });
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (sports.Any())
                        {
                            foreach (var sport in sports)
                                await _clubSportRepository.DeleteAsync(sport);
                        }

                        await _clubRepository.DeleteAsync(orig);
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
        }

        public async Task<RequestResponse<ClubSportViewModel>> AddSportOnClub(ClubSportViewModel clubDetailViewModel)
        {
            var noticable = new Notifiable();
            var viewModel = new ClubSportViewModel();
            try
            {


                var club = await _clubRepository.GetById(clubDetailViewModel.ClubUUId);
                if (club is null)
                    throw new Exception("Clube não encontrado!");

                var sport = await _sportsRepository.GetById(clubDetailViewModel.SportUUId);
                if (sport is null)
                    throw new Exception("Esporte não encontrado!");

                var exist = await _clubRepository.GetClubAndSportId(clubDetailViewModel.ClubUUId, clubDetailViewModel.SportUUId);
                if(exist != null && exist.Any())
                    throw new Exception("Esporte ja cadastrado!");

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var result = await _clubSportRepository.InsertAsync(new ClubSport(club.Id, sport.Id, clubDetailViewModel.UserId));
                        viewModel = _mapper.Map<ClubSportViewModel>(await _clubSportRepository.GetById(result.UUId));
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<ClubSportViewModel>(viewModel,noticable);
        }


        public async Task<NoContentResponse> RemoveSportOnClub(Guid Id, int UserId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _clubSportRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Esporte do Clube não encontrado!");

                orig.Disable(UserId);
                if (noticable.Valid)
                {

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {

                        await _clubSportRepository.DeleteAsync(orig);
                       

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
        }

        
    }
}
