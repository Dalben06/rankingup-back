using Dapper.Contrib.Extensions;
using RankingUp.Club.Domain.Entities;
using RankingUp.Core.Domain;
using RankingUp.Core.Extensions;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Player.Domain.Entities
{
    [Table("Players")]
    public class Players : AuditEntity
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Phone { get; private set; }

        [Computed]
        public ICollection<Clubs> Clubs { get; set; }

        [Computed]
        public ICollection<Sports> Sports { get; set; }


        public Players() : base(0)
        {
        }

        public Players(int userId, string name, string description, string phone) : base(userId)
        {
            UserId = userId;
            Name = name;
            Description = description;
            Phone = phone.OnlyNumbers();

            Validate();
        }

        public override void Disable(long IdUsuario)
        {
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
            IsDeleted = true;
        }

        public override void Validate() { }
    }
}
