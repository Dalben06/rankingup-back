using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Club.Domain.Entities
{
    [Table("ClubSports")]
    public class ClubSport : AuditEntity
    {
        public int ClubId { get; set; }
        public int SportId { get; set; }
        [Computed]
        public Clubs Club { get; set; }
        [Computed]
        public Sports Sport { get; set; }

        public ClubSport(): base(0)
        {

        }
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
