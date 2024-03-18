using AutoMapper;
using MySqlX.XDevAPI.Common;
using RankingUp.Background.Service.Service;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Domain;
using RankingUp.Core.ViewModels;
using RankingUp.Player.Domain.IRepositories;
using RankingUp.Tournament.Application.Interfaces;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.DomainServices;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Entities.Filters;
using RankingUp.Tournament.Domain.Events;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Services
{
    public class RankingAppService : IRankingAppService
    {
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentGameRepository _tournamentGameRepository;
        private readonly ITournamentTeamRepository _tournamentTeamRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _mediatorHandler;

        public RankingAppService(ITournamentsRepository tournamentsRepository,
            ITournamentGameRepository tournamentGameRepository,
            ITournamentTeamRepository tournamentTeamRepository,
            IClubRepository clubRepository,
            IMapper mapper,
            IMediatorHandler mediatorHandler)
        {
            _tournamentsRepository = tournamentsRepository;
            _tournamentGameRepository = tournamentGameRepository;
            _tournamentTeamRepository = tournamentTeamRepository;
            _clubRepository = clubRepository;
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

        public async Task<RequestResponse<PaginationViewModel<RankingDetailViewModel>>> GetRankingByFilter(TournamentFilter filter)
        {
            try
            {
                var result = this._mapper.Map<Pagination<Tournaments>, PaginationViewModel<RankingDetailViewModel>>(await _tournamentsRepository.GetTournamentsByFilter(filter));
                return new RequestResponse<PaginationViewModel<RankingDetailViewModel>>(
                    result
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<PaginationViewModel<RankingDetailViewModel>>(ex.Message);
            }
        }


        public async Task<RequestResponse<RankingDetailViewModel>> GetRanking(Guid Id)
        {
            
            try
            {
               
                var result = await _tournamentsRepository.GetById(Id);

                var gamesTask = _tournamentGameRepository.GetAllGamesByTournamentId(Id);
                var teamsTask = _tournamentTeamRepository.GetAllByTournament(Id);

                await Task.WhenAll(gamesTask, teamsTask);

                result.Games = await gamesTask;
                result.Teams = await teamsTask;
                var responseDetail = this._mapper.Map<RankingDetailViewModel>(result);
                if (result.IsRanking)
                {
                    var rankingDomainService = new RankingTeamDomainService(result.Games.ToList(), result.Teams.ToList());
                    responseDetail.Rankings = this._mapper.Map<List<RankingTeamViewModel>>(rankingDomainService.GetRankingTeams());
                }

                return new RequestResponse<RankingDetailViewModel>(responseDetail, new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<RankingDetailViewModel>(ex.Message);
            }
        }

        public async Task<RequestResponse<IEnumerable<RankingGameDetailViewModel>>> GetGames(Guid RankingId, bool IsFinished = false)
        {
            try
            {
                if (!IsFinished)
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


        public async Task<RequestResponse<IEnumerable<RankingDetailViewModel>>> GetRankingsByClub(Guid ClubId)
        {
            try
            {
                return new RequestResponse<IEnumerable<RankingDetailViewModel>>(
                    this._mapper.Map<IEnumerable<RankingDetailViewModel>>(await _tournamentsRepository.GetAllByClub(ClubId))
                    , new Notifiable());
            }
            catch (Exception ex)
            {
                return new RequestResponse<IEnumerable<RankingDetailViewModel>>(ex.Message);
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
                        QueueTaskEvent.Instance.AddTask(new RankingStartedEvent(rank.UUId, rank.CreatePersonId));

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

                if (orig.IsFinish)
                    noticable.AddNotification("Ranking já foi finalizado!");

                if (!orig.IsActive)
                    noticable.AddNotification("Ranking está inativo!");

                orig.StartEvent(UseId);
                if (noticable.Valid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(orig);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new RankingStartedEvent(orig.UUId, UseId));
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
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await _tournamentsRepository.UpdateAsync(orig);
                        scope.Complete();
                    }
                    QueueTaskEvent.Instance.AddTask(new RankingEndedEvent(orig.UUId, UseId));
                }
            }
            catch (Exception ex)
            {
                noticable.AddNotification(ex.Message);
            }
            return new NoContentResponse(noticable);
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
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
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
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
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



    }
}
