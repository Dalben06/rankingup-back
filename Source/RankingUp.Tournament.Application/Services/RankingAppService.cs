using AutoMapper;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Events;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Services
{
    public class RankingAppService : IRankingAppService
    {
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentTeamRepository _tournamentTeamRepository;
        private readonly ITournamentGameRepository _tournamentGameRepository;
        private readonly IRankingQueueRepository _rankingQueueRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerClubsRepository _playerClubsRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _mediatorHandler;

        public RankingAppService(ITournamentsRepository tournamentsRepository, ITournamentTeamRepository tournamentTeamRepository
            , ITournamentGameRepository tournamentGameRepository, IRankingQueueRepository rankingQueueRepository
            , IClubRepository clubRepository, IPlayerRepository playerRepository, IPlayerClubsRepository playerClubsRepository, IMapper mapper, IMediatorHandler mediatorHandler)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _rankingQueueRepository = rankingQueueRepository;
            _clubRepository = clubRepository;
            _playerRepository = playerRepository;
            _playerClubsRepository = playerClubsRepository;
            _mapper = mapper;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<RequestResponse<IEnumerable<RankingDetailViewModel>>> GetAllRankings()
        {
            try
            {
                return new RequestResponse<IEnumerable<RankingDetailViewModel>>(
                    this._mapper.Map<IEnumerable<RankingDetailViewModel>>(await _tournamentsRepository.GetAll(true))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingDetailViewModel>>(ex.Message);
            }
        }
        public async Task<RequestResponse<RankingDetailViewModel>> GetRanking(Guid Id)
        {
            try
            {
                return new RequestResponse<RankingDetailViewModel>(
                    this._mapper.Map<RankingDetailViewModel>(await _tournamentsRepository.GetById(Id))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<RankingDetailViewModel>(ex.Message);
            }
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
        public async Task<RequestResponse<IEnumerable<RankingGameDetailViewModel>>> GetGamesGoing(Guid RankingId)
        {
            try
            {
                return new RequestResponse<IEnumerable<RankingGameDetailViewModel>>(
                    this._mapper.Map<IEnumerable<RankingGameDetailViewModel>>(await _tournamentGameRepository.GetGameNotFinishTournamentId(RankingId))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingGameDetailViewModel>>(ex.Message);
            }
        }



        public async Task<RequestResponse<RankingDetailViewModel>> CreateRanking(RankingDetailViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var rank = _mapper.Map<Tournaments>(model);
                rank.SetClub(await _clubRepository.GetById(model.ClubUUId));

                if (model.SameInformationClub)
                    rank.SetClubInfoInRank();

                rank.Validate();
                noticable.AddNotifications(rank.Notifications);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        rank = await _tournamentsRepository.InsertAsync(rank);
                        scope.Complete();
                    }
                    if (rank.IsStart)
                        await _mediatorHandler.PublishDomainEvent(new RankingStartedEvent(rank.UUId, rank.CreatePersonId));

                    return new RequestResponse<RankingDetailViewModel>(_mapper.Map<RankingDetailViewModel>(await _tournamentsRepository.GetById(rank.Id)), noticable);
                }

            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingDetailViewModel>(noticable);
        }
        public async Task<NoContentResponse> StartRanking(Guid Id, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _tournamentsRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Ranking não encontrado!");

                if(orig.IsFinish)
                    noticable.AddNotification("Ranking já foi finalizado!");

                if (!orig.IsActive)
                    noticable.AddNotification("Ranking está inativo!");

                orig.StartEvent(UseId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(orig);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new RankingStartedEvent(orig.UUId, UseId));
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
        }
        public async Task<NoContentResponse> EndRanking(Guid Id, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _tournamentsRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Ranking não encontrado!");

                orig.FinishEvent(UseId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(orig);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new RankingEndedEvent(orig.UUId, UseId));
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
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

                if(team.Tournament != null && team.Tournament.OnlyClubMembers)
                {
                    var playersClub = await _playerClubsRepository.GetPlayerAndClubId(team.Tournament.ClubId, team.TeamId);
                    if(playersClub is null)
                        noticable.AddNotification("Esse Ranking somente permite jogadores associados ao clube");
                }

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        team = await _tournamentTeamRepository.InsertAsync(team);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new PlayerInRankingEvent(team.UUId,team.Tournament.UUId,model.UserId,Domain.Enums.PlayerRankingActionEnum.Added));
                    return new RequestResponse<RankingPlayerViewModel>(_mapper.Map<RankingPlayerViewModel>(await _tournamentTeamRepository.GetById(team.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingPlayerViewModel>(noticable);
        }
        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGame(RankingCreateGameViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var game = new TournamentGame(
                    (await _tournamentTeamRepository.GetById(model.TeamOneUUId)),
                    (await _tournamentTeamRepository.GetById(model.TeamTwoUUId)),
                    (await _tournamentsRepository.GetById(model.TournamentUUId)), model.UserId);

                game.Validate();

                noticable.AddNotifications(game.Notifications);

                if (game.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if(game.Tournament != null)
                {
                    var gamePlaying = await _tournamentGameRepository.GetAllGamesByTournamentId(game.Tournament.UUId);
                    if(gamePlaying !=null && gamePlaying.Count() >= game.Tournament.MatchSameTime)
                        noticable.AddNotification("O numero maximo de jogos ao mesmo tempo foi atingido");
                }

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        game = await _tournamentGameRepository.InsertAsync(game);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new RankingGameCreatedEvent(game.UUId, game.Tournament.UUId, game.TeamOne.UUId, game.TeamTwo.UUId,game.CreatePersonId));
                    return new RequestResponse<RankingGameDetailViewModel>(_mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(game.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingGameDetailViewModel>(noticable);
        }
        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGameUsingQueue(Guid TournamentId, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var playersInQueue = await this._rankingQueueRepository.GetByTournamentIdOrderByCreateDate(TournamentId);
                if (!playersInQueue.Any() && playersInQueue.Count() <= 1)
                    throw new Exception("Não há jogadores suficientes na Fila");

                var games = await this._tournamentGameRepository.GetAllGamesByTournamentId(TournamentId);
                var team1 = playersInQueue.First().Team;

                TournamentTeam team2 = null;
                int gamesPlayedWithTeam1 = int.MaxValue;
                foreach (var rankingQueue in playersInQueue.Where(x => x.TeamId != team1.Id))
                {
                    var gamesPlayeds = games.Where(x => x.TeamOneId == rankingQueue.TeamId || x.TeamTwoId == rankingQueue.TeamId);
                    if (!gamesPlayeds.Any())
                    {
                        gamesPlayedWithTeam1 = 0;
                        team2 = rankingQueue.Team;
                        break;
                    }
                    var gamesWithTeam1 = games.Where(x => x.TeamOneId == rankingQueue.TeamId || x.TeamTwoId == team1.Id).Count();
                    gamesWithTeam1 += games.Where(x => x.TeamTwoId == rankingQueue.TeamId || x.TeamOneId == team1.Id).Count();
                    
                    if(gamesWithTeam1 < gamesPlayedWithTeam1)
                    {
                        gamesPlayedWithTeam1 = gamesWithTeam1;
                        team2 = rankingQueue.Team;
                    }
                }

                var game = new TournamentGame(
                    team1,
                    team2,
                    (await _tournamentsRepository.GetById(TournamentId)), UseId);

                game.Validate();
                noticable.AddNotifications(game.Notifications);

                if (game.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if (game.Tournament != null)
                {
                    var gamePlaying = await _tournamentGameRepository.GetAllGamesByTournamentId(game.Tournament.UUId);
                    if (gamePlaying != null && gamePlaying.Count() >= game.Tournament.MatchSameTime)
                        noticable.AddNotification("O numero maximo de jogos ao mesmo tempo foi atingido");
                }

                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        game = await _tournamentGameRepository.InsertAsync(game);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new RankingGameCreatedEvent(game.UUId, game.Tournament.UUId, game.TeamOne.UUId, game.TeamTwo.UUId, game.CreatePersonId));
                    return new RequestResponse<RankingGameDetailViewModel>(_mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(game.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingGameDetailViewModel>(noticable);
        }



        public async Task<RequestResponse<RankingGameDetailViewModel>> UpdateGame(RankingGameDetailViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _tournamentGameRepository.GetById(model.UUId);
                if (orig is null)
                    throw new Exception("Jogo não encontrado!");

                var game = new TournamentGame(model.TeamOneGamePoints,model.TeamTwoGamePoints,
                    orig.TeamOne,
                    orig.TeamTwo,
                    (await _tournamentsRepository.GetById(orig.TournamentId)),
                    model.UserId, model.IsFinished);

                game.Validate();
                noticable.AddNotifications(game.Notifications);

                if (game.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if (noticable.Valid)
                {
                    game.Id = orig.Id;
                    game.UUId = orig.UUId;
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentGameRepository.UpdateAsync(game);
                        scope.Complete();
                    }
                    await _mediatorHandler.PublishDomainEvent(new RankingGameUpdatedEvent(game.UUId, game.Tournament.UUId, game.IsFinished, game.TeamOne.UUId, game.TeamTwo.UUId, game.UpdatePersonId));
                    return new RequestResponse<RankingGameDetailViewModel>(_mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(game.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingGameDetailViewModel>(noticable);
        }
        public async Task<RequestResponse<RankingDetailViewModel>> UpdateRanking(RankingDetailViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var rank = _mapper.Map<Tournaments>(model);
                var orig = await _tournamentsRepository.GetById(rank.UUId);

                if (orig is null)
                    noticable.AddNotification("Jogador não encontrado!");

                rank.SetClub(await _clubRepository.GetById(model.ClubUUId));
                if (model.SameInformationClub)
                    rank.SetClubInfoInRank();

                rank.Validate();
                noticable.AddNotifications(rank.Notifications);
                if (noticable.Valid)
                {
                    rank.Id = orig.Id;
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(rank);
                        scope.Complete();
                    }
                    return new RequestResponse<RankingDetailViewModel>(_mapper.Map<RankingDetailViewModel>(await _tournamentsRepository.GetById(rank.UUId)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingDetailViewModel>(noticable);
        }


        public async Task<NoContentResponse> RemoveRanking(Guid Id, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var orig = await _tournamentsRepository.GetById(Id);

                if (orig is null)
                    throw new Exception("Ranking não encontrado!");

                var GamePending = await _tournamentGameRepository.GetGameNotFinishTournamentId(orig.UUId);
                if (GamePending is not null && GamePending.Any())
                    noticable.AddNotification("Não é possivel finalizar um ranking com partidas em andamento!");

                orig.Disable(UseId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(orig);
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
