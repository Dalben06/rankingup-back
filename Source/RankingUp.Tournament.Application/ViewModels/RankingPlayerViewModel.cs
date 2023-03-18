using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Tournament.Application.ViewModels
{
    public class RankingPlayerViewModel :BaseViewModel
    {
        [Required(ErrorMessage = "Jogador é obrigatório")]
        public Guid PlayerUUId { get; set; }
        [Required(ErrorMessage = "Ranking é obrigatório")]
        public Guid TournamentUUId { get; set; }

        public string PlayerName { get; set; }
        public bool IsActive { get; set; }
    }
}
