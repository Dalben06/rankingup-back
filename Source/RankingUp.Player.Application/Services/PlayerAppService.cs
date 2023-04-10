using AutoMapper;
using RankingUp.Club.Domain.Entities;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Domain;
using RankingUp.Player.Application.ViewModel;
using RankingUp.Player.Domain.Entities;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Sport.Domain.Entities;
using RankingUp.Sport.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Player.Application.Services
{
    public class PlayerAppService : IPlayerAppService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerSportsRepository _playerSportsRepository;
        private readonly IPlayerClubsRepository _playerClubsRepository;
        private readonly IClubRepository _clubRepository;
        private readonly ISportsRepository _sportsRepository;
        private readonly IMapper _mapper;

        public PlayerAppService(IPlayerRepository playerRepository, IPlayerSportsRepository playerSportsRepository, IPlayerClubsRepository playerClubsRepository
            , IClubRepository clubRepository, ISportsRepository sportsRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _playerSportsRepository = playerSportsRepository;
            _playerClubsRepository = playerClubsRepository;
            _clubRepository = clubRepository;
            _sportsRepository = sportsRepository;
            _mapper = mapper;
        }

        public async Task<RequestResponse<PlayerViewModel>> GetPlayer(Guid Id)
        {
            try
            {
                return new RequestResponse<PlayerViewModel>(
                    this._mapper.Map<PlayerViewModel>(await _playerRepository.GetById(Id))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<PlayerViewModel>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<PlayerViewModel>>> GetPlayers()
        {
            try
            {
                return new RequestResponse<IEnumerable<PlayerViewModel>>(
                    this._mapper.Map<IEnumerable<PlayerViewModel>>(await _playerRepository.GetAll())
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<PlayerViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<PlayerViewModel>>> GetPlayersFromClubId(Guid Id)
        {
            try
            {
                return new RequestResponse<IEnumerable<PlayerViewModel>>(
                    this._mapper.Map<IEnumerable<PlayerViewModel>>(await _playerClubsRepository.GetPlayersFromClub(Id))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<PlayerViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<PlayerViewModel>> CreatePlayer(PlayerCreateViewModel playerModel)
        {
            var noticable = new Notifiable();
            try
            {
                var player = _mapper.Map<Players>(playerModel);
                noticable.AddNotifications(player.Notifications);
                Clubs club = null;
                Sports sport = null;

                if (playerModel.ClubUUId != Guid.Empty)
                    club = await _clubRepository.GetById(playerModel.ClubUUId);

                if (playerModel.SportUUId != Guid.Empty)
                    sport = await _sportsRepository.GetById(playerModel.SportUUId);
                else if(club != null && club.Sports != null && club.Sports.Any())
                    sport = club.Sports.FirstOrDefault();



                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        player = await _playerRepository.InsertAsync(player);
                        if(club != null)
                            await _playerClubsRepository.InsertAsync(new PlayerClubs(true, player.CreatePersonId, club, player));

                        if(sport != null) 
                            await _playerSportsRepository.InsertAsync(new PlayerSports(true, player.CreatePersonId, sport, player));

                        scope.Complete();
                    }
                    return new RequestResponse<PlayerViewModel>(_mapper.Map<PlayerViewModel>(await _playerRepository.GetById(player.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<PlayerViewModel>(noticable);
        }


        public async Task<RequestResponse<PlayerViewModel>> UpdatePlayer(PlayerViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var player = _mapper.Map<Players>(model);
                var orig = await _playerRepository.GetById(player.UUId);

                if (orig is null)
                    noticable.AddNotification("Jogador não encontrado!");

               
                noticable.AddNotifications(player.Notifications);
                if (noticable.Valid)
                {
                    player.Id = orig.Id;
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _playerRepository.UpdateAsync(player);
                        scope.Complete();
                    }
                    return new RequestResponse<PlayerViewModel>(_mapper.Map<PlayerViewModel>(await _playerRepository.GetById(player.UUId)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<PlayerViewModel>(noticable);
        }

        public async Task<RequestResponse<PlayerSportViewModel>> AddSport(PlayerSportViewModel model)
        {
            var noticable = new Notifiable();
            var viewModel = new PlayerSportViewModel();
            try
            {
                var player = await _playerRepository.GetById(model.PlayerUUId);
                if (player is null) throw new Exception("Jogador não encontrado!");

                var sport = await _sportsRepository.GetById(model.SportUUId);
                if (sport is null) throw new Exception("Esporte não encontrado!");

                var exist = await _playerSportsRepository.GetPlayerAndSportId(model.PlayerUUId, model.SportUUId);
                if (exist != null) throw new Exception("Esporte já registrado no Jogador!");

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var result = await _playerSportsRepository.InsertAsync(new PlayerSports(true, model.UserId, sport, player));
                        viewModel = _mapper.Map<PlayerSportViewModel>(await _playerSportsRepository.GetById(result.UUId));
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<PlayerSportViewModel>(viewModel, noticable);
        }

        public async Task<NoContentResponse> RemoveSport(Guid Id, int UserId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _playerSportsRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Esporte do Jogador não encontrado!");

                orig.Disable(UserId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _playerSportsRepository.DeleteAsync(orig);
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

        public async Task<RequestResponse<PlayerClubViewModel>> AddClub(PlayerClubViewModel model)
        {
           var noticable = new Notifiable();
            var viewModel = new PlayerClubViewModel();
            try
            {
                var player = await _playerRepository.GetById(model.PlayerUUId);
                if (player is null) throw new Exception("Jogador não encontrado!");

                var club = await _clubRepository.GetById(model.ClubUUId);
                if (club is null) throw new Exception("Clube não encontrado!");

                var exist = await  _playerClubsRepository.GetPlayersAndClubId(model.PlayerUUId, model.ClubUUId);
                if(exist != null) throw new Exception("Jogador já cadastro ao Clube!");


                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var result = await _playerClubsRepository.InsertAsync(new PlayerClubs(true, model.UserId, club, player));
                        viewModel = _mapper.Map<PlayerClubViewModel>(await _playerClubsRepository.GetById(result.UUId));
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<PlayerClubViewModel>(viewModel, noticable);
        }

        public async Task<NoContentResponse> RemoveClub(Guid Id, int UserId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _playerClubsRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Clube do Jogador não encontrado!");

                orig.Disable(UserId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _playerClubsRepository.DeleteAsync(orig);
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
