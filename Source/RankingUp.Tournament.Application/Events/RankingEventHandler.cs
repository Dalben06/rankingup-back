using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RankingUp.Tournament.Application.Hubs;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Enums;
using RankingUp.Tournament.Domain.Events;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Events
{
    public class RankingEventHandler : INotificationHandler<PlayerInRankingEvent>,
                                       INotificationHandler<RankingStartedEvent>,
                                       INotificationHandler<RankingEndedEvent>,
                                       INotificationHandler<RankingGameCreatedEvent>,
                                       INotificationHandler<RankingGameUpdatedEvent>
    {

        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentTeamRepository _tournamentTeamRepository;
        private readonly ITournamentGameRepository _tournamentGameRepository;
        private readonly IRankingQueueRepository _rankingQueueRepository;
        private readonly IRankingGameService _rankingGameService;
        private readonly IHubContext<RankingHub> _hubContext;
        private readonly IMapper _mapper;

        public RankingEventHandler(ITournamentsRepository tournamentsRepository,
                                   ITournamentTeamRepository tournamentTeamRepository, 
                                   ITournamentGameRepository tournamentGameRepository, 
                                   IRankingQueueRepository rankingQueueRepository, 
                                   IHubContext<RankingHub> hubContext, 
                                   IMapper mapper,
                                   IRankingGameService rankingGameService)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _rankingQueueRepository = rankingQueueRepository;
            _hubContext = hubContext;
            _mapper = mapper;
            _rankingGameService = rankingGameService;
        }

        public async Task Handle(PlayerInRankingEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var team = await _tournamentTeamRepository.GetById(notification.UUId);
                var tournament = await _tournamentsRepository.GetById(notification.TournamentUUId);

                if (notification.Action == PlayerRankingActionEnum.Added)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _rankingQueueRepository.InsertAsync(new RankingQueue(tournament, team, notification.UserId));
                        scope.Complete();
                    }
                }
                else
                {
                    var playerInQueue = (await _rankingQueueRepository.GetByTournamentIdOrderByCreateDate(notification.TournamentUUId))
                        ?.FirstOrDefault(x => x.Team.UUId == notification.TournamentUUId);

                    if (playerInQueue != null)
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                        {
                            playerInQueue.Disable(notification.UserId);
                            await _rankingQueueRepository.DeleteAsync(new RankingQueue(tournament, team, notification.UserId));
                            scope.Complete();
                        }
                    }

                }

                var signalrEvent = new RankingUpdateSignalr(
                    notification.UUId,
                    notification.Action == PlayerRankingActionEnum.Added ? SignalrRankingEventType.PlayerAdded : SignalrRankingEventType.PlayerRemoved,
                    notification.Action == PlayerRankingActionEnum.Added
                    ? _mapper.Map<RankingPlayerViewModel>(await _tournamentTeamRepository.GetById(notification.UUId))
                    : new RankingPlayerViewModel { UUId = notification.UUId, TournamentUUId = notification.TournamentUUId });


                await this._hubContext.Clients.Groups(notification.TournamentUUId.ToString().ToLower()).SendAsync("rankingUpdate", signalrEvent);

                if (tournament.AutoQueue && notification.Action == PlayerRankingActionEnum.Added && tournament.IsStart && !tournament.IsFinish)
                    await _rankingGameService.CreateGameUsingQueue(notification.TournamentUUId, notification.UserId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Handle(RankingStartedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _tournamentsRepository.GetById(notification.UUId);
                var signalrEvent = new RankingUpdateSignalr(
                   notification.UUId,
                   SignalrRankingEventType.RankingStarted,
                   _mapper.Map<RankingDetailViewModel>(tournament));
                await this._hubContext.Clients.Group(notification.UUId.ToString().ToLower()).SendAsync("rankingUpdate", signalrEvent);

                if (tournament.AutoQueue )
                {
                    var playersInQueue = await this._rankingQueueRepository.GetByTournamentIdOrderByCreateDate(notification.UUId);
                    if (playersInQueue.Any())
                    {
                        for (int i = 0; i < playersInQueue.Count(); i += 2)
                            await _rankingGameService.CreateGameUsingQueue(notification.UUId, notification.UserId);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Handle(RankingEndedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _tournamentsRepository.GetById(notification.UUId);
                var signalrEvent = new RankingUpdateSignalr(
                   notification.UUId,
                   SignalrRankingEventType.RankingFinished,
                   _mapper.Map<RankingDetailViewModel>(tournament));
                await this._hubContext.Clients.Groups(notification.UUId.ToString().ToLower()).SendAsync("rankingUpdate", signalrEvent);
                

                var playersInQueue = await this._rankingQueueRepository.GetByTournamentIdOrderByCreateDate(notification.UUId);
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var playerQueue in playersInQueue)
                    {
                        playerQueue.Disable(notification.UserId);
                        await _rankingQueueRepository.DeleteAsync(playerQueue);
                    }
                    scope.Complete();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Handle(RankingGameCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {

                var signalrEvent = new RankingUpdateSignalr(
                   notification.UUId,
                   SignalrRankingEventType.GameCreated,
                   _mapper.Map<RankingGameDetailViewModel>(await _tournamentGameRepository.GetById(notification.UUId)));

                await this._hubContext.Clients.Groups(notification.TournamentId.ToString().ToLower()).SendAsync("rankingUpdate", signalrEvent);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Handle(RankingGameUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var tournament = await _tournamentsRepository.GetById(notification.TournamentId);
                var game = await _tournamentGameRepository.GetById(notification.UUId);
                var signalrEvent = new RankingUpdateSignalr(
                   notification.UUId,
                   notification.IsFinished ? SignalrRankingEventType.GameFinished : SignalrRankingEventType.GameUpdated,
                   _mapper.Map<RankingGameDetailViewModel>(game));

                await this._hubContext.Clients.Groups(notification.TournamentId.ToString().ToLower()).SendAsync("rankingUpdate", signalrEvent);

                if (notification.IsFinished && notification.NeedCreateMatch)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _rankingQueueRepository.InsertAsync(new RankingQueue(tournament, game.TeamOne, notification.UserId));
                        await _rankingQueueRepository.InsertAsync(new RankingQueue(tournament, game.TeamTwo, notification.UserId));
                        scope.Complete();
                    }
                    
                    if(tournament.AutoQueue && tournament.IsStart && !tournament.IsFinish && notification.NeedCreateMatch)
                        await _rankingGameService.CreateGameUsingQueue(notification.TournamentId, notification.UserId);
                }
                

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
