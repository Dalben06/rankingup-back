using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Player.Application.ViewModel
{
    public class PlayerClubViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Jogador é obrigatório")]
        public Guid PlayerUUId { get; set; }
        [Required(ErrorMessage = "O Clube é obrigatório")]
        public Guid ClubUUId { get; set; }

    }
}
