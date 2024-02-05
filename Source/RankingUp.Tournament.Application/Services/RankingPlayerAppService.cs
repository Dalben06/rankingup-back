using AutoMapper;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
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

        public RankingPlayerAppService(ITournamentsRepository tournamentsRepository,
                                       ITournamentTeamRepository tournamentTeamRepository,
                                       ITournamentGameRepository tournamentGameRepository,
                                       IPlayerRepository playerRepository,
                                       IPlayerClubsRepository playerClubsRepository,
                                       IRankingQueueRepository rankingQueueRepository,
                                       IMapper mapper,
                                       IMediatorHandler mediatorHandler)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _playerRepository = playerRepository;
            _playerClubsRepository = playerClubsRepository;
            _rankingQueueRepository = rankingQueueRepository;
            _mapper = mapper;
            _mediatorHandler = mediatorHandler;
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
                var team = new TournamentTeam
                    (await _tournamentsRepository.GetById(model.TournamentUUId),
                    await _playerRepository.GetById(model.PlayerUUId), true, model.UserId);

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
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        team = await _tournamentTeamRepository.InsertAsync(team);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new PlayerInRankingEvent(team.UUId, team.Tournament.UUId, model.UserId, Domain.Enums.PlayerRankingActionEnum.Added));
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
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentTeamRepository.DeleteAsync(orig);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new PlayerInRankingEvent(orig.UUId, orig.Tournament.UUId, UseId, Domain.Enums.PlayerRankingActionEnum.Deleted));
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
