using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Tournament.Application.ViewModels
{
    public class RankingDetailViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Nome Ranking é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome do Ranking pode ter no máximo {1} caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O Descrição do Ranking é obrigatório")]
        [StringLength(250, ErrorMessage = "O Descrição do Ranking pode ter no máximo {1} caracteres")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Utiliza as mesma informações do clube é obrigatório")]
        public bool SameInformationClub { get; set; }
        public string Address { get; set; }
        public string AddressNumber { get; set; }
        public string AddressComplement { get; set; }
        public string AddressDistrict { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        [StringLength(200, ErrorMessage = "O Email pode ter no máximo {1} caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A Data do Ranking é obrigatória")]
        public DateTime EventDate { get; set; }
        [Required(ErrorMessage = "A hora de inicio é obrigatório")]
        public string EventHourStart { get; set; }
        [Required(ErrorMessage = "A hora de termino é obrigatório")]
        public string EventHourEnd { get; set; }
        [Required(ErrorMessage = "O Clube é obrigatório")]
        public Guid ClubUUId { get; set; }
        [Required(ErrorMessage = "Somente Membro do clube é obrigatório")]
        public bool OnlyClubMembers { get; set; }
        [Required(ErrorMessage = "O Preço é obrigatório")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "O Preço para alunos é obrigatório")]
        public decimal MemberPrice { get; set; }
        [Required(ErrorMessage = "Quantidade de partidas ao mesmo tempo é obrigatório")]
        public int MatchSameTime { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool AutoQueue { get; set; }
        public bool HasNotificationToPlayer { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinish { get; set; }
        public decimal? Distance { get; set; }

        public string ClubName { get; set; }

        public IEnumerable<RankingPlayerViewModel> Teams { get; set; }
        public IEnumerable<RankingGameDetailViewModel> Games { get; set; }

    }
}
