using RankingUp.Core.Messages;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingGameCreatedEvent : DomainEvent
    {
        public RankingGameCreatedEvent(Guid UUId, Guid tournamentId, Guid playerOneId, Guid playerTwoId, int UserId):base(UUId,UserId)
        {
            TournamentId = tournamentId;
            PlayerOneId = playerOneId;
            PlayerTwoId = playerTwoId;
        }

        public Guid TournamentId { get; private set; }
        public Guid PlayerOneId { get; private set; }
        public Guid PlayerTwoId { get; private set; }

    }
}
