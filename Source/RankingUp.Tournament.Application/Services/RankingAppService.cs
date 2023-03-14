using AutoMapper;
using RankingUp.Club.Domain.IRepositories;
using RankingUp.Core.Domain;
using RankingUp.Tournament.Application.ViewModels;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;
using System.Transactions;

namespace RankingUp.Tournament.Application.Services
{
    public class RankingAppService : IRankingAppService
    {
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public RankingAppService(ITournamentsRepository tournamentsRepository, IClubRepository clubRepository, IMapper mapper)
        {
            _tournamentsRepository = tournamentsRepository;
            _clubRepository = clubRepository;
            _mapper = mapper;
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

        
    }
}
