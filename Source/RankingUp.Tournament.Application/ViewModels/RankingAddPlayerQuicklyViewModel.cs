using RankingUp.Core.ViewModels;

namespace RankingUp.Tournament.Application.ViewModels
{
    public class RankingAddPlayerQuicklyViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public Guid? ClubUUId { get; set; }
        public Guid TournamentUUId { get; set; }

    }
}
