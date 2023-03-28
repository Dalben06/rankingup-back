using RankingUp.Core.Messages;
using RankingUp.Tournament.Domain.Enums;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingUpdateSignalr : Event
    {
        public string EventType { get; set; }
        public object Data { get; set; }
        public RankingUpdateSignalr(Guid UUId, SignalrRankingEventType type, object data)
        {
            this.UUId = UUId;
            this.EventType = type.ToString();
            this.Data = data;
        }

    }
}
