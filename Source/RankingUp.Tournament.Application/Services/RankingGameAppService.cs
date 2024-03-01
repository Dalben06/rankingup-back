using AutoMapper;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;
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
        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGame(RankingCreateGameViewModel model)
        {
            var noticable = new Notifiable();
            try
            {
                var teamOne = _tournamentTeamRepository.GetById(model.TeamOneUUId);
                var teamTwo = _tournamentTeamRepository.GetById(model.TeamTwoUUId);
                var ranking = _tournamentsRepository.GetById(model.TournamentUUId);

                await Task.WhenAll(teamOne, teamTwo, ranking);

                var game = new TournamentGame(
                    await teamOne,
                    await teamTwo,
                    await ranking, model.UserId);

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

        public async Task<RequestResponse<RankingGameDetailViewModel>> CreateGameUsingQueue(Guid TournamentId, int UseId)
        {
            var noticable = new Notifiable();
            try
            {
                var playersInQueue = await this._rankingQueueRepository.GetByTournamentIdOrderByCreateDate(TournamentId);
                if (!playersInQueue.Any() || playersInQueue.Count() <= 1)
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

                    if (gamesWithTeam1 < gamesPlayedWithTeam1)
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
    }
}
