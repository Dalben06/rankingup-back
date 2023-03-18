using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;

namespace RankingUp.Tournament.Domain.Entities
{
    public class TournamentGame : AuditEntity
    {

        public TournamentGame():base(0)
        {

        }

        // new Game
        public TournamentGame(TournamentTeam teamOne, TournamentTeam teamTwo, Tournaments tour, int UserId) : base(UserId)
        {
            IsFinished = false;
            SetTeamOne(teamOne);
            SetTeamTwo(teamTwo);
            SetTournament(tour);
        }

        // new Update/Finish game
        public TournamentGame(int teamOneGamePoints, int teamTwoGamePoints
            , TournamentTeam teamOne, TournamentTeam teamTwo,Tournaments tour, int UserId, bool isFinished ): base(UserId)
        {
            TeamOneGamePoints = teamOneGamePoints;
            TeamTwoGamePoints = teamTwoGamePoints;
            IsFinished = isFinished;
            SetTeamOne(teamOne);
            SetTeamTwo(teamTwo);
            SetTournament(tour);
        }

        public int TournamentId { get; private set; }
        public int TeamOneId { get; private set; }
        public int TeamTwoId { get; private set; }
        public int TeamOneGamePoints { get; private set; }
        public int TeamTwoGamePoints { get; private set; }
        public bool IsFinished { get; private set; }
        [Computed]
        public TournamentTeam TeamOne { get; private set; }
        [Computed]
        public TournamentTeam TeamTwo { get; private set; }
        [Computed]
        public Tournaments Tournament { get; private set; }
        [Computed]
        public TournamentTeam Winner
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (this.TeamOneGamePoints > this.TeamTwoGamePoints)
                    return TeamOne;
                else if (this.TeamOneGamePoints < this.TeamTwoGamePoints)
                    return TeamTwo;
                else
                    return null;
            }
        }
        [Computed]
        public TournamentTeam Loser
        {
            get
            {
                if (!IsFinished)
                    return null;

                if (this.TeamOneGamePoints > this.TeamTwoGamePoints)
                    return TeamTwo;
                else if (this.TeamOneGamePoints < this.TeamTwoGamePoints)
                    return TeamOne;
                else
                    return null;
            }
        }

        public void SetTournament(Tournaments tour)
        {
            if (tour == null)
            {
                this.AddNotification("Campeonato/Ranking invalido!");
                return;
            }

            TournamentId = tour.Id;
            Tournament = tour;
        }

        public void SetTeamOne(TournamentTeam Team)
        {
            if(Team == null)
            {
                this.AddNotification("Time 1 invalido!");
                return;
            }

            TeamOneId= Team.Id;
            TeamOne= Team;
        }

        public void SetTeamTwo(TournamentTeam Team)
        {
            if (Team == null)
            {
                this.AddNotification("Time 2 invalido!");
                return;
            }

            TeamTwoId = Team.Id;
            TeamTwo = Team;
        }

        public override void Disable(long IdUsuario)
        {
            IsDeleted = true;
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
        }

        public override void Validate()
        {
            if (this.IsFinished && this.TeamOneGamePoints == this.TeamTwoGamePoints)
                this.AddNotification("Não pode finalizar um Jogo com a mesma pontuação os dois times");
        }

       

    }
}
