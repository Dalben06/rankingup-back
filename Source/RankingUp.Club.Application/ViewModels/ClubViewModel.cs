
using RankingUp.Core.ViewModels;

namespace RankingUp.Club.Application.ViewModels
{
    public class ClubViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompleteAddress { get; set; }
        public string BusinessHourEnd { get; set; }
        public string BusinessHourStart { get; set; }
    }
}
