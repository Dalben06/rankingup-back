using RankingUp.Tournament.Domain.Entities;
using System.Linq;

namespace RankingUp.Tournament.Domain.DomainServices
{
    public class RankingTeamDomainService
    {
        private readonly List<TournamentGame> _games;
        private readonly List<TournamentTeam> _teams;

        public RankingTeamDomainService(List<TournamentGame> games, List<TournamentTeam> teams)
        {
            _games = games;
            _teams = teams;
        }


        public List<RankingTeam> GetRankingTeams()
        {
            var rankingTeams = _teams.Select(t => { return new RankingTeam(t); }).ToList();

            foreach (var game in _games.Where(g => g.IsFinished))
            {
                var winner = rankingTeams.FirstOrDefault(r => r.TeamId == game.Winner.TeamId);
                var loser = rankingTeams.FirstOrDefault(r => r.TeamId == game.Loser.TeamId);
                if (winner is null || loser is null) continue;

                winner.AddWin(game.WinnerPoints);
                loser.AddLose(game.LoserPoints);
            }
            return rankingTeams.OrderByDescending(r => r.Wins).ToList();
        }


    }
}
