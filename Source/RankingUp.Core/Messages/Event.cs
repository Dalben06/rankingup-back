using MediatR;

namespace RankingUp.Core.Messages
{
    public abstract class Event : Message, INotification
    {
        protected Event()
        {
        }
    }
}
