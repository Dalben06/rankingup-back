using RankingUp.Core.Messages;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingGameUpdatedEvent : DomainEvent
    {
        public RankingGameUpdatedEvent(Guid UUId, Guid tournamentId, bool isFinished, Guid teamOneId, Guid teamTwoId, int UserId, bool needCreateMatch) : base(UUId, UserId)
        {
            TournamentId = tournamentId;
            IsFinished = isFinished;
            TeamOneId = teamOneId;
            TeamTwoId = teamTwoId;
            NeedCreateMatch = needCreateMatch;
        }

        public Guid TournamentId { get; private set; }
        public bool IsFinished { get; private set; }
        public Guid TeamOneId { get; private set; }
        public Guid TeamTwoId { get; private set; }
        public bool NeedCreateMatch { get; private set; }
    }
}
