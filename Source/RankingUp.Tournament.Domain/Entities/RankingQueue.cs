using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Player.Domain.Entities;

namespace RankingUp.Tournament.Domain.Entities
{
    public class RankingQueue : AuditEntity
    {
        public int TournamentId { get; private set; }
        public int TeamId { get; private set; }

        public RankingQueue(): base(0)
        {

        }
        public RankingQueue(Tournaments tournaments, TournamentTeam team, int UserId ): base(UserId)
        {
            SetTeam(team);
            SetTournament(tournaments);
        }

        [Computed]
        public Tournaments Tournament { get; set; }
        [Computed]
        public TournamentTeam Team { get; set; }
        public override void Disable(long IdUsuario)
        {
            IsDeleted = true;
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
        }

        public void SetTournament(Tournaments tour)
        {
            if (tour == null)
                this.AddNotification("Torneio não encontrado!");

            this.Tournament = tour;
            this.TournamentId = tour?.Id ?? 0;
        }

        public void SetTeam(TournamentTeam Team)
        {
            if (Team == null)
                this.AddNotification("Jogador não encontrado!");

            this.Team = Team;
            this.TeamId = Team?.Id ?? 0;
        }

        public override void Validate()
        {


        }

    }
}
