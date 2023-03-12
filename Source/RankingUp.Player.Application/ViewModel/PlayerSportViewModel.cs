using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Player.Application.ViewModel
{
    public class PlayerSportViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Jogador é obrigatório")]
        public Guid PlayerUUId { get; set; }
        [Required(ErrorMessage = "O Esporte é obrigatório")]
        public Guid SportUUId { get; set; }

    }
}
