using Microsoft.Extensions.DependencyInjection;
using RankingUp.Background.Service.Interfaces;
using RankingUp.Core.Communication.Mediator;
using RankingUp.Core.Messages;

namespace RankingUp.Background.Service.Service
{
    public sealed class RunEventTaskService : IRunEventTaskService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RunEventTaskService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<bool> RunAsync<T>(T @event) where T : Event
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                await mediator.PublishEvent(@event);
                return true;
            }
        }
    }
}
