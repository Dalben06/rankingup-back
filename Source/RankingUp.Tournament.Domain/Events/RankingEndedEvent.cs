using RankingUp.Core.Messages;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingEndedEvent : DomainEvent
    {
        public RankingEndedEvent(Guid UUId, int UserId) : base(UUId, UserId)
        {
        }
    }
}
