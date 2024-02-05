using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Tournament.Application.ViewModels
{
    public class RankingGameDetailViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Jogos do Time 1 é obrigatório")]
        public int TeamOneGamePoints { get; set; }
        [Required(ErrorMessage = "Jogos do Time 2 é obrigatório")]
        public int TeamTwoGamePoints { get; set; }
        [Required(ErrorMessage = "Jogo finalizado é obrigatório")]
        public bool IsFinished { get; set; }



        public string TeamOnePlayerName { get; set; }
        public string TeamTwoPlayerName { get; set; }
        public string TournamentName { get; set; }
        public string TournamentDescription { get; set; }

        public string WinnerPlayerName { get; set; }
        public string LoserPlayerName { get; set; }

        // TODO retornar a lista de Matches q tiveram

        public string TitleGame => $"{TeamOnePlayerName} vs {TeamTwoPlayerName}";
        public string TitleGamePoints => $"{TeamOneGamePoints} - {TeamTwoGamePoints}";

    }
}
