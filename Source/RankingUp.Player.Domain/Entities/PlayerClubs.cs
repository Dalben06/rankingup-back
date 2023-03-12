using Dapper.Contrib.Extensions;
using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Domain;

namespace RankingUp.Player.Domain.Entities
{
    [Table("PlayerClubs")]
    public class PlayerClubs : AuditEntity
    {
        public int PlayerId { get; private set; }
        public int ClubId { get; private set; }
        public bool IsActive { get; private set; }

        public PlayerClubs(): base(0)
        {

        }
        public PlayerClubs(bool IsActive, int UserId, Clubs club, Players player) : base(UserId)
        {
            this.IsActive= IsActive;
            SetPlayer(player);
            SetClub(club);
            Validate();
        }

        [Computed]
        public Clubs Club { get; private set; }

        [Computed]
        public Players Player { get; private set; }

        private void SetClub(Clubs Club)
        {
            this.Club = Club;
            this.ClubId = Club?.Id ?? 0;
        }

        private void SetPlayer(Players Player)
        {
            this.Player = Player;
            this.PlayerId = Player?.Id ?? 0;
        }


        public override void Disable(long IdUsuario)
        {
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
            IsActive = false;
        }

        public override void Validate() { }
    
    }
}
