using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Models;

namespace ConsoleApp1
{
    internal class Program
    {
        private static ConcurrentDictionary<Guid, GameInfo> _games { get; set; }
        private static List<GameInfo> _historyGames { get; set; }

        private static Object m_Lock = new Object();

        private static void Main(string[] args)
        {
            _games = new ConcurrentDictionary<Guid, GameInfo>();
            _historyGames = new List<GameInfo>();
            var gameUpdate = new Progress<GameInfo>();

            gameUpdate.ProgressChanged += (senderOfProgressChanged, item) =>
            {
                if (!item.IsFinished)
                {
                    if (_games.TryAdd(item.GameId, item))
                    {
                        return;
                    }
                    _games[item.GameId] = item;
                }

                _games.TryRemove(item.GameId, out _);
                _historyGames.Add(item);

                OutputGames();
            };

            var gameSource = new GameDataSource();
            Parallel.Invoke(new ParallelOptions(), () => gameSource.RunGameProcess(gameUpdate));

            Console.ReadKey();
        }

        private static void OutputGames()
        {
            bool acquiredLock = false;

            try
            {
                Monitor.Enter(m_Lock, ref acquiredLock);
                Console.Clear();

                Console.WriteLine("Current games \n");
                foreach (var value in _games.Values)
                {
                    Console.WriteLine(@$"{value.HomeTeam} {value.HomeTeamScore} - {value.AwayTeam} {value.AwayTeamScore}");
                }

                Console.WriteLine("\nCompleted games");
                foreach (var item in _historyGames.OrderBy(it => (it.HomeTeam, it.HomeTeamScore, it.CreationDate)))
                {
                    Console.WriteLine(@$"{item.HomeTeam} {item.HomeTeamScore} - {item.AwayTeam} {item.AwayTeamScore} | {item.CreationDate}");
                }
            }
            finally
            {
                if (acquiredLock)
                {
                    Monitor.Exit(m_Lock);
                }
            }

        }
    }
}
