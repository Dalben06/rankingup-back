using RankingUp.Core.Domain;

namespace RankingUp.Club.Domain.Entities
{
    public class ClubSport : AuditEntity
    {
        public int ClubId { get; set; }
        public int SportId { get; set; }

        public ClubSport(int clubId, int sportId, int UserId): base(UserId)
        {
            ClubId = clubId;
            SportId = sportId;
        }

        public override void Disable(long IdUsuario)
        {
            this.IsDeleted = true;
            this.DeleteDate = this.UpdateDate = DateTime.UtcNow;
            this.DeletePersonId = this.UpdatePersonId = (int)IdUsuario;
        }

        public override void Validate()
        {
           
        }
    }
}
