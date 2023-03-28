using RankingUp.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingUp.Tournament.Domain.Events
{
    public class RankingGameUpdatedEvent : DomainEvent
    {
        public RankingGameUpdatedEvent(Guid UUId, Guid tournamentId, bool isFinished, Guid teamOneId, Guid teamTwoId, int UserId): base(UUId,UserId)
        {
            TournamentId = tournamentId;
            IsFinished = isFinished;
            TeamOneId = teamOneId;
            TeamTwoId = teamTwoId;
        }

        public Guid TournamentId { get; private set; }
        public bool IsFinished { get; private set; }
        public Guid TeamOneId { get; private set; }
        public Guid TeamTwoId { get; private set; }
    }
}
