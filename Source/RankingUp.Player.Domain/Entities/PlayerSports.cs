using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Player.Domain.Entities
{
    [Table("PlayerSports")]
    public class PlayerSports : AuditEntity
    {
        public int PlayerId { get; private set; }
        public int SportId { get; private set; }
        public bool IsActive { get; private set; }

        public PlayerSports() : base(0)
        {

        }
        public PlayerSports(bool IsActive, int UserId, Sports sport, Players player) : base(UserId)
        {
            this.IsActive = IsActive;
            SetPlayer(player);
            SetSport(sport);
            Validate();
        }

        [Computed]
        public Sports Sport { get; private set; }

        [Computed]
        public Players Player { get; private set; }

        private void SetSport(Sports Sport)
        {
            this.Sport = Sport;
            this.SportId = Sport?.Id ?? 0;
        }

        private void SetPlayer(Players Player)
        {
            this.Player = Player;
            this.PlayerId = Player?.Id ?? 0;
        }

        public override void Disable(long IdUsuario)
        {
            DeleteDate = UpdateDate = DateTime.Now;
            IsDeleted = true;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
            IsActive = false;
        }

        public override void Validate() { }
    }
}
