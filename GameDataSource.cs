using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleApp1.Models;

namespace ConsoleApp1
{
    public class GameDataSource
    {
        private Dictionary<Guid, GameInfo> _games { get; set; }

        private Random _random = new Random();

        public GameDataSource()
        {
            _games = new Dictionary<Guid, GameInfo>();
        }

        public void RunGameProcess(IProgress<GameInfo> progress, int updateSpeed = 600)
        {
            while (true)
            {
                Thread.Sleep(updateSpeed);
                var needUpdateCurrentGame = _random.Next(2);
                if (needUpdateCurrentGame != default)
                {
                    UpdateActiveGame(progress);
                }

                var needAddGame = _random.Next(2);
                if (needAddGame == default)
                {
                    continue;
                }

                AddNewGame(progress);
            }
        }

        private void AddNewGame(IProgress<GameInfo> progress)
        {
            if (_games.Count >= 10)
            {
                return;
            }
            var counties = new[]
            {
                "Uruguay", "Italy", "Spain", "Mexico", "Canada",
                "Brazil","Argentina","Australia","Germany","France"
            };
            var countyForInsert = counties.OrderBy(x => _random.Next()).Take(2);
            var newGame = new GameInfo();
            newGame.AwayTeam = countyForInsert.First();
            newGame.HomeTeam = countyForInsert.Last();
            _games.TryAdd(newGame.GameId, newGame);
            progress.Report(newGame);
        }
        private void UpdateActiveGame(IProgress<GameInfo> progress)
        {
            if (_games.Count == default)
            {
                return;
            }

            var gameId = _random.Next(_games.Count);

            var gameInfo = _games.Values.ElementAt(gameId);
            gameInfo.IsFinished = _random.Next(15) < 1;
            gameInfo.AwayTeamScore = _random.Next(2) == default
                ? gameInfo.AwayTeamScore
                : ++gameInfo.AwayTeamScore;
            gameInfo.HomeTeamScore = _random.Next(2) == default
                ? gameInfo.HomeTeamScore
                : ++gameInfo.HomeTeamScore;

            if (gameInfo.IsFinished)
            {
                _games.Remove(gameInfo.GameId);
                progress.Report(gameInfo);
                return;
            }

            _games[gameInfo.GameId] = gameInfo;
            progress.Report(gameInfo);
        }
    }
}
