using MediatR;
using RankingUp.Core.Domain;
using RankingUp.Core.Messages;

namespace RankingUp.Core.Communication.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishDomainEvent<T>(T notificacao) where T : DomainEvent
        {
            await _mediator.Publish<T>(notificacao);
        }

        public async Task PublishEvent<T>(T evento) where T : Event
        {
            await _mediator.Publish<T>(evento);
        }

        public async Task<Notifiable> SendCommand<T>(T comando) where T : Command
        {
            return await _mediator.Send(comando);
        }
    }
}
