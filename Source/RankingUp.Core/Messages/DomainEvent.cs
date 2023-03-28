namespace RankingUp.Core.Messages
{
    public abstract class DomainEvent : Event
    {
        public int UserId { get; set; }
        public DomainEvent(Guid UUId, int userId) : base()
        {
            this.UUId = UUId;
            UserId = userId;
        }
    }
}
