using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Player.Domain.Entities;

namespace RankingUp.Tournament.Domain.Entities
{
    public class TournamentTeam : AuditEntity
    {
        public TournamentTeam() : base(0)
        {

        }
        public TournamentTeam(Tournaments tournament, Players player,bool IsActive, int UserId) : base(UserId)
        {
            SetTournament(tournament);
            SetTeam(player);
            this.IsActive= IsActive;
        }

        public int TournamentId { get; private set; }
        public int TeamId { get; private set; }
        public bool IsActive { get; private set; }

        [Computed]
        public Tournaments Tournament { get; set; }
        [Computed]
        public Players Player { get; set; }

        public void ActivePlayer()
        {
            IsActive = true;
        }

        public void InativePlayer(int UserId)
        {
            IsActive = false;
            UpdateDate = DateTime.UtcNow;
            UpdatePersonId = UserId;
        }

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

        public void SetTeam(Players players)
        {
            if (players == null)
                this.AddNotification("Jogador não encontrado!");

            this.Player = players;
            this.TeamId = players?.Id ?? 0;
        }

        public override void Validate() 
        {
           
        
        }
    }
}
