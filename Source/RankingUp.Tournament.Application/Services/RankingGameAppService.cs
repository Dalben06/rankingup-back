using AutoMapper;
using RankingUp.Background.Service.Interfaces;
using RankingUp.Background.Service.Service;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.DomainServices;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Events;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Services
{
    public class RankingGameAppService : IRankingGameService
    {

        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentTeamRepository _tournamentTeamRepository;
        private readonly ITournamentGameRepository _tournamentGameRepository;
        private readonly IRankingQueueRepository _rankingQueueRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _mediatorHandler;

        public RankingGameAppService(ITournamentsRepository tournamentsRepository,
                                     ITournamentTeamRepository tournamentTeamRepository,
                                     ITournamentGameRepository tournamentGameRepository,
                                     IRankingQueueRepository rankingQueueRepository,
                                     IMapper mapper,
                                     IMediatorHandler mediatorHandler)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _rankingQueueRepository = rankingQueueRepository;
            _mapper = mapper;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<RequestResponse<IEnumerable<RankingGameDetailViewModel>>> GetGames(Guid RankingId, bool? IsFinished)
        {
            try
            {
                if (IsFinished.HasValue && !IsFinished.GetValueOrDefault())
                    return new RequestResponse<IEnumerable<RankingGameDetailViewModel>>(
                   this._mapper.Map<IEnumerable<RankingGameDetailViewModel>>(await _tournamentGameRepository.GetGameNotFinishTournamentId(RankingId))
                   , new Notifiable());


                return new RequestResponse<IEnumerable<RankingGameDetailViewModel>>(
                    this._mapper.Map<IEnumerable<RankingGameDetailViewModel>>(await _tournamentGameRepository.GetAllGamesByTournamentId(RankingId))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingGameDetailViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<RankingTeamViewModel>>> GetRankingPlayers(Guid RankingId)
        {
            try
            {
                var games = _tournamentGameRepository.GetAllGamesByTournamentId(RankingId);
                var teams = _tournamentTeamRepository.GetAllByTournament(RankingId);

                await Task.WhenAll(games, teams);
                var domainService = new RankingTeamDomainService((await games).ToList(), (await teams).ToList());


                return new RequestResponse<IEnumerable<RankingTeamViewModel>>(
                    this._mapper.Map<IEnumerable<RankingTeamViewModel>>(domainService.GetRankingTeams())
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingTeamViewModel>>(ex.Message);
            }
        }

        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGame(RankingCreateGameViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var teamOneTask = _tournamentTeamRepository.GetById(model.TeamOneUUId);
                var teamTwoTask = _tournamentTeamRepository.GetById(model.TeamTwoUUId);
                var rankingTask = _tournamentsRepository.GetById(model.TournamentUUId);

                await Task.WhenAll(teamOneTask, teamTwoTask, rankingTask);

                var teamOne = await teamOneTask;
                var teamTwo = await teamTwoTask;
                var ranking = await rankingTask;

                var game = new TournamentGame(
                    teamOne,
                    teamTwo,
                    ranking, model.UserId);

                game.Validate();

                noticable.AddNotifications(game.Notifications);

                if (game.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if (teamOne != null && teamTwo != null && teamOne.Id == teamTwo.Id)
                    noticable.AddNotification("Os times da partida devem ser diferentes");

                if (ranking != null && (teamOne?.TournamentId != ranking.Id || teamTwo?.TournamentId != ranking.Id))
                    noticable.AddNotification("Os times devem pertencer ao Ranking informado");

                if (game.Tournament != null)
                {
                    var gamePlaying = await _tournamentGameRepository.GetGameNotFinishTournamentId(game.Tournament.UUId);
                    if (gamePlaying != null && gamePlaying.Count() >= game.Tournament.MatchSameTime)
                        noticable.AddNotification("O numero maximo de jogos ao mesmo tempo foi atingido");
                }

                if (noticable.Valid)
                {
                    var playersInQueue = await _rankingQueueRepository.GetByTournamentIdOrderByCreateDate(model.TournamentUUId);
                    var playersToDeleteInQueue = playersInQueue
                        .Where(player => player.TeamId == teamOne.Id || player.TeamId == teamTwo.Id)
                        .ToList();

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var playerQueue in playersToDeleteInQueue)
                        {
                            playerQueue.Disable(model.UserId);
                            await _rankingQueueRepository.DeleteAsync(playerQueue);
                        }
                        game = await _tournamentGameRepository.InsertAsync(game);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new RankingGameCreatedEvent(game.UUId, game.Tournament.UUId, game.TeamOne.UUId, game.TeamTwo.UUId, game.CreatePersonId));
                    return new RequestResponse<RankingGameDetailViewModel>(_mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(game.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingGameDetailViewModel>(noticable);
        }

        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGameUsingQueue(Guid TournamentId, int UserId)
        {
            var noticable = new Notifiable();
            try
            {
                var playersInQueueTask = _rankingQueueRepository.GetByTournamentIdOrderByCreateDate(TournamentId);
                var gamesTask = _tournamentGameRepository.GetAllGamesByTournamentId(TournamentId);
                var gamesPlayingTask = _tournamentGameRepository.GetGameNotFinishTournamentId(TournamentId);
                var rankingTask = _tournamentsRepository.GetById(TournamentId);

                await Task.WhenAll(playersInQueueTask, gamesTask, gamesPlayingTask, rankingTask);

                var playersInQueue = (await playersInQueueTask).ToList();
                if (playersInQueue.Count <= 1)
                    throw new Exception("Não há jogadores suficientes na Fila");

                var games = await gamesTask;
                var gamesPlaying = await gamesPlayingTask;
                var ranking = await rankingTask;
                var team1 = playersInQueue.First().Team;

                TournamentTeam team2 = null;
                int gamesPlayedWithTeam1 = int.MaxValue;
                foreach (var rankingQueue in playersInQueue.Where(x => x.TeamId != team1.Id))
                {
                    var gamesWithTeam1 = games.Count(x =>
                        (x.TeamOneId == team1.Id && x.TeamTwoId == rankingQueue.TeamId) ||
                        (x.TeamTwoId == team1.Id && x.TeamOneId == rankingQueue.TeamId));

                    if (gamesWithTeam1 < gamesPlayedWithTeam1)
                    {
                        gamesPlayedWithTeam1 = gamesWithTeam1;
                        team2 = rankingQueue.Team;
                    }
                }

                var game = new TournamentGame(
                    team1,
                    team2,
                    ranking, UserId);

                game.Validate();
                noticable.AddNotifications(game.Notifications);

                if (game.Tournament?.IsFinish ?? false)
                    noticable.AddNotification("O Ranking já foi finalizado");

                if (team2 == null || team1.Id == team2.Id)
                    noticable.AddNotification("Não há times diferentes suficientes na Fila");

                if (ranking != null && (team1.TournamentId != ranking.Id || team2?.TournamentId != ranking.Id))
                    noticable.AddNotification("Os times devem pertencer ao Ranking informado");

                if (game.Tournament != null)
                {
                    if (gamesPlaying != null && gamesPlaying.Count() >= game.Tournament.MatchSameTime)
                        noticable.AddNotification("O numero maximo de jogos ao mesmo tempo foi atingido");
                }

                if (noticable.Valid)
                {
                    var playerToDeleteInQueue = playersInQueue
                        .Where(player => player.TeamId == team1.Id || player.TeamId == team2.Id)
                        .ToList();
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var playerQueue in playerToDeleteInQueue)
                        {
                            playerQueue.Disable(UserId);
                            await _rankingQueueRepository.DeleteAsync(playerQueue);
                        }
                        game = await _tournamentGameRepository.InsertAsync(game);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new RankingGameCreatedEvent(game.UUId, game.Tournament.UUId, game.TeamOne.UUId, game.TeamTwo.UUId, game.CreatePersonId));
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

                var game = new TournamentGame(model.TeamOneGamePoints, model.TeamTwoGamePoints,
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
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentGameRepository.UpdateAsync(game);
                        scope.Complete();
                    }

                    QueueTaskEvent.Instance.AddTask(new RankingGameUpdatedEvent(game.UUId, game.Tournament.UUId,
                            game.IsFinished, game.TeamOne.UUId, game.TeamTwo.UUId, game.UpdatePersonId, !orig.IsFinished && game.IsFinished));

                    return new RequestResponse<RankingGameDetailViewModel>(_mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(game.Id)), noticable);
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new RequestResponse<RankingGameDetailViewModel>(noticable);
        }

        
    }
}
