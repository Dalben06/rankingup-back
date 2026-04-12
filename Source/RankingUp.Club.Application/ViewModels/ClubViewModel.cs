
using RankingUp.Core.ViewModels;

namespace RankingUp.Club.Application.ViewModels
{
    public class ClubViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompleteAddress { get; set; }
        public TimeOnly BusinessHourEnd { get; set; }
        public TimeOnly BusinessHourStart { get; set; }
    }
}
