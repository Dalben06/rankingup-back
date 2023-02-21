using RankingUp.Core.DataAnnotations;

namespace RankingUp.Core.Domain
{
    public abstract class AuditEntity : BaseEntity
    {
        [OnlyInsert]
        public DateTimeOffset CreateDate { get; set; }
        [OnlyInsert]
        public int CreatePersonId { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public int UpdatePersonId { get; set; }
        [OnlyUpdate]
        public bool IsDeleted { get; set; }
        [OnlyUpdate]
        public DateTimeOffset? DeleteDate { get; set; }
        [OnlyUpdate]
        public int? DeletePersonId { get; set; }



    }
}
