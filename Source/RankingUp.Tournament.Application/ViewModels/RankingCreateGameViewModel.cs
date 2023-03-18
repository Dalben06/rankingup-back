using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Tournament.Application.ViewModels
{
    public class RankingCreateGameViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Jogos do Time 1 é obrigatório")]
        public int TeamOneGamePoints { get; set; }
        [Required(ErrorMessage = "Jogos do Time 2 é obrigatório")]
        public int TeamTwoGamePoints { get; set; }
        [Required(ErrorMessage = "Time 1 é obrigatório")]
        public Guid TeamOneUUId { get; set; }
        [Required(ErrorMessage = "Time 2 é obrigatório")]
        public Guid TeamTwoUUId { get; set; }
        [Required(ErrorMessage = "Ranking é obrigatório")]
        public Guid TournamentUUId { get; set; }


        public string TeamOnePlayerName { get; set; }
        public string TeamTwoPlayerName { get; set; }

    }
}
