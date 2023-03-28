using RankingUp.Core.Messages;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingStartedEvent : DomainEvent
    {
        public RankingStartedEvent(Guid UUId, int UserId) : base(UUId, UserId)
        {
        }
    }
}
