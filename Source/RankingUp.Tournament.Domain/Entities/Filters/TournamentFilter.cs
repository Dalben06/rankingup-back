using RankingUp.Core.Domain;

namespace RankingUp.Tournament.Domain.Entities.Filters
{
    public class TournamentFilter : Filter
    {
        public bool IsRanking { get; set; }
        public decimal? RadioDistance { get; set; }
        public bool? OnlyFinished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
