using RankingUp.Core.Domain;
using RankingUp.Core.Messages;

namespace RankingUp.Core.Communication.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T evento) where T : Event;
        Task<Notifiable> SendCommand<T>(T comando) where T : Command;
        Task PublishDomainEvent<T>(T notificacao) where T : DomainEvent;

    }
}
