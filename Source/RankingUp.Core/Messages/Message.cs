namespace RankingUp.Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public Guid UUId { get; protected set; }
        public DateTime Timestamp { get; set; }

        protected Message()
        {
            MessageType = GetType().Name;
            Timestamp = DateTime.UtcNow;
        }
    }
}
