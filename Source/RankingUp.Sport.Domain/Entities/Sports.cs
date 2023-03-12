using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;

namespace RankingUp.Sport.Domain.Entities
{
    [Table("Sports")]
    public class Sports : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public bool IsEnable { get; private set; }

        // To Do pensar em criar um IENumerable<SportRule> para colocar as regras do sports

        public override void Disable(long IdUsuario) { }


        public override void Validate()
        {

        }
    }
}
