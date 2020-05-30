using System;
using System.Collections;
using System.Collections.Generic;

namespace Connect4
{
    class Program
    {
        private static string _player1 = "Player 1";
        private static string _player2 = "Player 2";
        private static ConsoleColor _defaultColor = ConsoleColor.Gray;
        private static ConsoleColor _player1Color = ConsoleColor.DarkRed;
        private static ConsoleColor _player2Color = ConsoleColor.DarkYellow;
        private static ConsoleColor _boardColor = ConsoleColor.DarkBlue;

        static void Main(string[] args)
        {
            writeSegments(new[]{
                ("Welcome to Connect 4!  Please enter player names.", _defaultColor)
            });

            Console.WriteLine("");

            writeSegments(new[]
            {
                ("Enter ", _defaultColor),
                ("Player 1 ", _player1Color),
                ("Name: ", _defaultColor)
            });

            _player1 = Console.ReadLine();

            Console.WriteLine($"Welcome {_player1}!");

            writeSegments(new[]
            {
                ("Enter ", _defaultColor),
                ("Player 2 ", _player2Color),
                ("Name: ", _defaultColor)
            });

            _player2 = Console.ReadLine();
            Console.WriteLine($"Welcome {_player2}!");

            var score = new Dictionary<string, int> {
                { _player1, 0},
                { _player2, 0 }
            };

            var quit = false;
            while (quit == false)
            {
                var board = new Connect4Game();
                printBoard(board.Board);

                Console.WriteLine("");

                while (board.WinCoords().Count == 0 && !board.IsDraw())
                {
                    var currentPlayer = _player1;
                    if (board.LastPlayer == _player1) currentPlayer = _player2;

                    writeSegments(new[]
                    {
                    (currentPlayer, currentPlayer == _player1 ? _player1Color : _player2Color),
                    ("'s turn.  Select a column to place your piece: ", _defaultColor)
                });

                    if (int.TryParse(Console.ReadLine(), out int x))
                    {
                        try
                        {
                            printBoard(board.Place(currentPlayer, x));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Invalid move. Try again.");
                        }

                        Console.WriteLine("");

                    }
                    else
                    {
                        //wrong input
                    }
                }

                if (board.IsDraw())
                {
                    Console.WriteLine($"Draw!");
                }
                else
                {
                    Console.WriteLine($"Congratulations {board.LastPlayer}!");
                    score[board.LastPlayer] = score[board.LastPlayer] + 1;
                }

                Console.WriteLine($"{_player1}: {score[_player1]}");
                Console.WriteLine($"{_player2}: {score[_player2]}");
                Console.WriteLine("");

                string again = string.Empty;
                while (again != "y" && again != "n")
                {
                    Console.WriteLine("Play again? (y/n)");
                    again = Console.ReadLine();
                }

                quit = again == "n";
            }

        }

        private static void printBoard(string[,] board)
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    Console.ForegroundColor = _boardColor;
                    Console.Write("[");

                    var occupant = board[row, col];
                    if (occupant == _player1) Console.ForegroundColor = _player1Color;
                    if (occupant == _player2) Console.ForegroundColor = _player2Color;
                    Console.Write(string.IsNullOrEmpty(occupant) ? " " : "O");

                    Console.ForegroundColor = _boardColor;
                    Console.Write("]");

                    Console.ForegroundColor = _defaultColor;
                }

                Console.WriteLine();
            }
        }

        private static void writeSegments((string, ConsoleColor)[] segments)
        {
            foreach (var s in segments)
            {
                Console.ForegroundColor = s.Item2;
                Console.Write(s.Item1);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    class Connect4Game
    {
        private HashSet<string> _players = new HashSet<string>();
        private int _playerCount;
        private int _playCount;
        private int _contigousWinCondition;
        private readonly (int, int) UP_LEFT = (1, -1);
        private readonly (int, int) UP = (1, 0);
        private readonly (int, int) UP_RIGHT = (1, 1);
        private readonly (int, int) LEFT = (0, -1);
        private readonly (int, int) RIGHT = (0, 1);
        private readonly (int, int) DOWN_LEFT = (-1, -1);
        private readonly (int, int) DOWN = (-1, 0);
        private readonly (int, int) DOWN_RIGHT = (-1, 1);

        public string[,] Board { get; }

        public string LastPlayer { get; private set; }

        public Connect4Game(int columnCount = 7, int rowCount = 6, int playerCount = 2, int contiguousWinCondition = 4)
        {
            Board = new string[rowCount, columnCount];
            _playerCount = playerCount;
            _contigousWinCondition = contiguousWinCondition;
        }

        public bool IsDraw()
        {
            return _playCount == Board.GetLength(0) * Board.GetLength(1);
        }

        public string[,] Place(string player, int column)
        {
            _players.Add(player);
            if (_players.Count > _playerCount)
            {
                throw new Exception($"Player {player} is invalid!");
            }

            return place(player, column);
        }

        public List<(int, int)> WinCoords()
        {
            /*
             * wins are determined by looking for an adjacent piece and then extending a line until
             * not finding that player's piece, hitting win condition, or hitting board boundary
             * 
             * 1 2 3
             * 4 p 6
             * 7 8 9 
             * 
             * 1 = row + 1, column + -1
             * 2 = row + 1, column + 0
             * 3 = row + 1, column + 1
             * 4 = row + 0, column + -1
             * 6 = row + 0, column + 1
             * 7 = row - 1, column + -1
             * 8 = row - 1, column + 0
             * 9 = row - 1, column + 1
             */
            List<(int, int)> winCoordList = new List<(int, int)>();
            if (_playCount < (_contigousWinCondition * 2 - 1)) return winCoordList;

            var cond = _contigousWinCondition;
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    if (getAtCoord(row, col) == LastPlayer)
                    {
                        if ((winCoordList = winCoords(winCoordList, row, col, UP_LEFT.Item1, UP_LEFT.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, UP.Item1, UP.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, UP_RIGHT.Item1, UP_RIGHT.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, LEFT.Item1, LEFT.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, RIGHT.Item1, RIGHT.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, DOWN_LEFT.Item1, DOWN_LEFT.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, DOWN.Item1, DOWN.Item2)).Count == cond ||
                            (winCoordList = winCoords(winCoordList, row, col, DOWN_RIGHT.Item1, DOWN_RIGHT.Item2)).Count == cond
                           )
                        {
                            return winCoordList;
                        }
                    }
                }
            }

            return winCoordList;
        }

        private List<(int, int)> winCoords(List<(int, int)> winCoords, int row, int col, int rowDir, int colDir)
        {
            if (getAtCoord(row, col) == LastPlayer)
            {
                winCoords.Add((row, col));

                if (winCoords.Count != _contigousWinCondition)
                    winCoords = this.winCoords(winCoords, row + rowDir, col + colDir, rowDir, colDir);
            }

            return winCoords.Count == _contigousWinCondition ? winCoords : new List<(int, int)>();
        }

        private string getAtCoord(int row, int col)
        {
            if (row < 0 || col < 0 || row >= Board.GetLength(0) || col >= Board.GetLength(1)) return null;

            return Board[row, col];
        }

        private string[,] place(string player, int column, int row = 0)
        {
            var coords = (Board.GetLength(0) - 1 - row, column - 1);

            if (string.IsNullOrEmpty(Board[coords.Item1, coords.Item2]))
            {
                Board[coords.Item1, coords.Item2] = player;
                LastPlayer = player;
                _playCount += 1;

                return Board;
            }

            return place(player, column, row + 1);
        }
    }
}
