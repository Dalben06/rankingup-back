using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Player.Application.ViewModel
{
    public class PlayerCreateViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Nome do Jogador é obrigatório")]
        [StringLength(50, ErrorMessage = "O Nome do Jogador pode ter no máximo {1} caracteres")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "O Descrição pode ter no máximo {1} caracteres")]
        public string Description { get; set; }
        [Required(ErrorMessage = "O Telefone do Jogador é obrigatório")]
        [StringLength(20, ErrorMessage = "O Telefone do Jogador pode ter no máximo {1} caracteres")]
        public string Phone { get; set; }

        public Guid ClubUUId { get; set; }
        public Guid SportUUId { get; set; }
    }
}
