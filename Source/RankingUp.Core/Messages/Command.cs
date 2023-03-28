using MediatR;
using RankingUp.Core.Domain;

namespace RankingUp.Core.Messages
{
    public abstract class Command : Message, IRequest<Notifiable>
    {
        public Notifiable Notification { get; set; }

        protected Command()
        {
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
