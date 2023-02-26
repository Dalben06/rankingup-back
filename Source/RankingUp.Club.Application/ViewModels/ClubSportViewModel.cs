using System.ComponentModel.DataAnnotations;

namespace RankingUp.Club.Application.ViewModels
{
    public class ClubSportViewModel
    {
        [Key]
        public Guid UUId { get; set; }
        [Required(ErrorMessage = "O Esporte é obrigatório")]
        public Guid SportUUId { get; set; }
        [Required(ErrorMessage = "O Clube é obrigatório")]
        public Guid ClubUUId { get; set; }

        public int UserId { get; set; }
    }
}
