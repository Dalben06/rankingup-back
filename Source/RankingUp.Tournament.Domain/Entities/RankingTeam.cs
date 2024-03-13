namespace RankingUp.Tournament.Domain.Entities
{
    public class RankingTeam
    {
        public int TeamId { get; private set; }
        public int Wins { get; private set; }
        public int Loses { get; private set; }
        public int Games { get; private set; }
        public int BalancePoints { get; private set; }
        public TournamentTeam Team { get; private set; }
        public RankingTeam(TournamentTeam team)
        {
            TeamId = team.TeamId;
            Team = team;
            Wins = 0;
            Loses = 0;
            Games = 0;
            BalancePoints = 0;
        }

        public void AddWin(int points)
        {
            this.Wins++;
            this.BalancePoints += points;
            this.Games++;
        }

        public void AddLose(int points)
        {
            this.Loses++;
            this.BalancePoints -= points;
            this.Games++;
        }


    }
}
