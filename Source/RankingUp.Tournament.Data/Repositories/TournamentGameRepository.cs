using RankingUp.Core.Data;
using RankingUp.Player.Domain.Entities;
using RankingUp.Tournament.Domain.Entities;
using RankingUp.Tournament.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingUp.Tournament.Data.Repositories
{
    public class TournamentGameRepository : ITournamentGameRepository
    {

        private readonly IBaseRepository _baseRepository;

        public TournamentGameRepository(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        // Todo verificar quando tiver o Times
        private string GetDefaultSql()
        {
            return @"
             SELECT TournamentGames.*, TeamOne.*,PlayerOne.*,TeamTwo.*, PlayerTwo.*
             FROM TournamentGames

             INNER JOIN Tournaments 
             ON Tournaments.Id = TournamentGames.TournamentId
             AND Tournaments.IsDeleted = 0

             INNER JOIN TournamentTeams as TeamOne
             ON TeamOne.Id = TournamentGames.TeamOneId
             AND TeamOne.IsDeleted = 0

             INNER JOIN Players as PlayerOne
             ON PlayerOne.Id = TeamOne.TeamId
             AND PlayerOne.IsDeleted = 0

             INNER JOIN TournamentTeams as TeamTwo
             ON TeamTwo.Id = TournamentGames.TeamTwoId
             AND TeamTwo.IsDeleted = 0

             INNER JOIN Players as PlayerTwo
             ON PlayerTwo.Id = TeamTwo.TeamId
             AND PlayerTwo.IsDeleted = 0

             WHERE 1 = 1
             AND TournamentGames.IsDeleted = 0

             ";
        }


        public Task<IEnumerable<TournamentGame>> GetAllGamesByTournamentId(Guid Id)
            => _baseRepository.GetAsync<TournamentGame, TournamentTeam, Players, TournamentTeam, Players>(GetDefaultSql() + " AND Tournaments.UUId = @Id ORDER BY TournamentGames.Id DESC", SQLMap(), new { Id });

        public async Task<TournamentGame?> GetGameNotFinishByTeamAndTournamentId(Guid tour, Guid teamId)
        {
            var SQL = GetDefaultSql() + @"  AND TournamentGames.IsFinished = 0 AND Tournaments.UUId = @tour
            AND ( TeamTwo.UUId = @teamId OR TeamOne.UUId = @teamId )
            ";

           return ( await _baseRepository.GetAsync<TournamentGame, TournamentTeam, Players, TournamentTeam, Players>(SQL, SQLMap(), new { tour, teamId }))?.FirstOrDefault();
        }

        public Task<IEnumerable<TournamentGame>> GetGameNotFinishTournamentId(Guid tour)
        {
            var SQL = GetDefaultSql() + @"  AND TournamentGames.IsFinished = 0 AND Tournaments.UUId = @tour ";

            return _baseRepository.GetAsync<TournamentGame, TournamentTeam, Players, TournamentTeam, Players>(SQL, SQLMap(), new { tour });
        }

        public Task<TournamentGame> GetById(Guid Id) => _baseRepository.GetByIdAsync<TournamentGame, TournamentTeam, Players, TournamentTeam, Players>(GetDefaultSql(), Id, SQLMap());
        public Task<TournamentGame> GetById(int Id) => _baseRepository.GetByIdAsync<TournamentGame, TournamentTeam,Players, TournamentTeam, Players>(GetDefaultSql(), Id, SQLMap());
        public Task<TournamentGame> InsertAsync(TournamentGame entity) => _baseRepository.InsertAsync<TournamentGame>(entity);
        public Task<bool> UpdateAsync(TournamentGame entity) => _baseRepository.UpdateAsync<TournamentGame>(entity);
        public Task<bool> DeleteAsync(TournamentGame entity) => _baseRepository.DeleteAsync<TournamentGame>(entity);

        private Func<TournamentGame, TournamentTeam, Players, TournamentTeam, Players, TournamentGame> SQLMap()
        {
            var dic = new Dictionary<long, TournamentGame>();
            return (game, teamOne, playerOne, teamTwo, playerTwo) =>
            {
                if (dic.TryGetValue(game.Id, out var existingGame))
                {
                    game = existingGame;
                }
                else
                {
                    teamOne.SetTeam(playerOne);
                    teamTwo.SetTeam(playerTwo);
                    game.SetTeamOne(teamOne);
                    game.SetTeamTwo(teamTwo);
                    dic.Add(game.Id, game);
                }

                return game;
            };

        }

       
    }
}
