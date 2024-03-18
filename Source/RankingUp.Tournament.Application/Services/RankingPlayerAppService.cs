using AutoMapper;
using RankingUp.Background.Service.Service;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
using RankingUp.Core.Extensions;
using RankingUp.Player.Application.Services;
using RankingUp.Player.Application.ViewModel;
using RankingUp.Player.Domain.Entities;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Events;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Services
{
    public class RankingPlayerAppService : IRankingPlayerService
    {
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentTeamRepository _tournamentTeamRepository;
        private readonly ITournamentGameRepository _tournamentGameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerClubsRepository _playerClubsRepository;
        private readonly IRankingQueueRepository _rankingQueueRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IPlayerAppService _playerAppService;

        public RankingPlayerAppService(ITournamentsRepository tournamentsRepository,
                                       ITournamentTeamRepository tournamentTeamRepository,
                                       ITournamentGameRepository tournamentGameRepository,
                                       IPlayerRepository playerRepository,
                                       IPlayerClubsRepository playerClubsRepository,
                                       IRankingQueueRepository rankingQueueRepository,
                                       IMapper mapper,
                                       IMediatorHandler mediatorHandler,
                                       IPlayerAppService playerAppService)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _playerRepository = playerRepository;
            _playerClubsRepository = playerClubsRepository;
            _rankingQueueRepository = rankingQueueRepository;
            _mapper = mapper;
            _mediatorHandler = mediatorHandler;
            _playerAppService = playerAppService;
        }

        public async Task<RequestResponse<IEnumerable<RankingPlayerViewModel>>> GetPlayers(Guid RankingId)
        {
            try
            {
                return new RequestResponse<IEnumerable<RankingPlayerViewModel>>(
                    this._mapper.Map<IEnumerable<RankingPlayerViewModel>>(await _tournamentTeamRepository.GetAllByTournament(RankingId))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingPlayerViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<RankingPlayerQueueViewModel>>> GetPlayersOnQueue(Guid RankingId)
        {
            try
            {
                return new RequestResponse<IEnumerable<RankingPlayerQueueViewModel>>(
                    this._mapper.Map<IEnumerable<RankingPlayerQueueViewModel>>(await _rankingQueueRepository.GetByTournamentIdOrderByCreateDate(RankingId))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingPlayerQueueViewModel>>(ex.Message);
            }
        }
        public async Task<RequestResponse<RankingPlayerViewModel>> AddPlayer(RankingPlayerViewModel model)
        {
            var noticable = new Notifiable();
            try
            {

                var ranking = _tournamentsRepository.GetById(model.TournamentUUId);
                var player = _playerRepository.GetById(model.PlayerUUId);

                await Task.WhenAll(ranking,player);
                var team = new TournamentTeam(await ranking,await player, true, model.UserId);

                team.Validate();
                noticable.AddNotifications(team.Notifications);
                if (team.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if (team.Tournament != null && team.Tournament.OnlyClubMembers)
                {
                    var playersClub = await _playerClubsRepository.GetPlayerAndClubId(team.Tournament.ClubId, team.TeamId);
                    if (playersClub is null)
                        noticable.AddNotification("Esse Ranking somente permite jogadores associados ao clube");
                }

                var playerInRanking = await _tournamentTeamRepository.GetAllByTournament(model.TournamentUUId);
                if (playerInRanking.Any(x => x.Player.UUId == model.PlayerUUId))
                    noticable.AddNotification("Jogador já cadastro no Ranking");


                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        team = await _tournamentTeamRepository.InsertAsync(team);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new PlayerInRankingEvent(team.UUId, team.Tournament.UUId, model.UserId, Domain.Enums.PlayerRankingActionEnum.Added));
                    return new RequestResponse<RankingPlayerViewModel>(_mapper.Map<RankingPlayerViewModel>(await _tournamentTeamRepository.GetById(team.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingPlayerViewModel>(noticable);
        }

        public async Task<NoContentResponse> RemovePlayer(Guid Id, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _tournamentTeamRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Jogador não encontrado!");

                var playerIsPlaying = await _tournamentGameRepository.GetGameNotFinishByTeamAndTournamentId(orig.Tournament.UUId, orig.UUId);
                if (playerIsPlaying != null)
                    noticable.AddNotification("Não pode remover um jogador em uma partida em andamento!");

                orig.Disable(UseId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentTeamRepository.DeleteAsync(orig);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new PlayerInRankingEvent(orig.UUId, orig.Tournament.UUId, UseId, Domain.Enums.PlayerRankingActionEnum.Deleted));
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
        }

        public async Task<RequestResponse<RankingPlayerViewModel>> AddPlayerQuickly(RankingAddPlayerQuicklyViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var addPlayerModel = new RankingPlayerViewModel
                {
                    IsActive = true,
                    TournamentUUId = model.TournamentUUId,
                    UserId = model.UserId,
                };

                var existPlayerByNumber = await _playerRepository.GetByPhoneNumber(model.PhoneNumber.OnlyNumbers());
                if(existPlayerByNumber != null)
                {
                    addPlayerModel.PlayerUUId = existPlayerByNumber.UUId;
                    return await AddPlayer(addPlayerModel);
                }

                var modelCreatePlayer = new PlayerCreateViewModel 
                {
                    ClubUUId = model.ClubUUId ?? Guid.Empty,
                    Name = model.Name,
                    Phone = model.PhoneNumber,
                    UserId = model.UserId,
                    Description = model.Description,
                };
                var resultPlayerCreate = await _playerAppService.CreatePlayer(modelCreatePlayer);
                noticable.AddNotifications(resultPlayerCreate.Notificacoes.Notifications);

                if (noticable.Invalid) return new RequestResponse<RankingPlayerViewModel>(noticable);

                addPlayerModel.PlayerUUId = resultPlayerCreate.Model.UUId;
                return await AddPlayer(addPlayerModel);
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingPlayerViewModel>(noticable);
        }
    }
}
