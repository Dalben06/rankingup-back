using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Player.Application.ViewModel
{
    public class PlayerCreateViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Nome do Jogador é obrigatório")]
        [StringLength(50, ErrorMessage = "O Nome do Jogador pode ter no máximo {1} caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O Descrição é obrigatório")]
        [StringLength(100, ErrorMessage = "O Descrição pode ter no máximo {1} caracteres")]
        public string Description { get; set; }
    }
}
