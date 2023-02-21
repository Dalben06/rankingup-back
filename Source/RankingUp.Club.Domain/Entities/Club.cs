using RankingUp.Core.Domain;

namespace RankingUp.Club.Domain.Entities
{
    public sealed class Club : AuditEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTimeOffset BusinessHourStart { get; set; }
        public DateTimeOffset BusinessHourEnd { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string ImageUrl { get; set; }

        public override void Disable(long IdUsuario)
        {
            return;
        }

        public override void Validate()
        {
            return;
        }
    }
}
