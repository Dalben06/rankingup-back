using RankingUp.Club.Application.ViewModels;
using RankingUp.Core.ViewModels;
using RankingUp.Sport.Application.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Player.Application.ViewModel
{
    public class PlayerViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Nome do Jogador é obrigatório")]
        [StringLength(50, ErrorMessage = "O Nome do Jogador pode ter no máximo {1} caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O Descrição é obrigatório")]
        [StringLength(100, ErrorMessage = "O Descrição pode ter no máximo {1} caracteres")]
        public string Description { get; set; }
        public IEnumerable<ClubViewModel> Clubs { get; set; }
        public IEnumerable<SportViewModel> Sports { get; set; }

    }
}
