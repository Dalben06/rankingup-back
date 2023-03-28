using RankingUp.Core.Messages;
using RankingUp.Tournament.Domain.Enums;

namespace RankingUp.Tournament.Domain.Events
{
    public class PlayerInRankingEvent : DomainEvent
    {
        public PlayerInRankingEvent(Guid UUId, Guid tournamentUUId, int UserId, PlayerRankingActionEnum playerRankingActionEnum) : base(UUId, UserId)
        {
            TournamentUUId = tournamentUUId;
            Action = playerRankingActionEnum;
        }

        public Guid TournamentUUId { get; private set; }
        public PlayerRankingActionEnum Action { get; private set; }
    }
}
