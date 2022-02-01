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
        private static ConsoleColor _winColor = ConsoleColor.DarkGreen;
        private static Connect4Game _game;

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
                _game = new Connect4Game();
                //_game = new Connect4Game(12, 11, 2, 6);
                printBoard(_game.Board);

                Console.WriteLine("");

                while (_game.WinCoords().Count == 0 && !_game.IsDraw())
                {
                    var currentPlayer = _player1;
                    if (_game.LastPlayer == _player1) currentPlayer = _player2;

                    writeSegments(new[]
                                        {
                                            (currentPlayer, currentPlayer == _player1 ? _player1Color : _player2Color),
                                            ("'s turn.  Select a column to place your piece: ", _defaultColor)
                                         });

                    if (int.TryParse(Console.ReadLine(), out int x))
                    {
                        try
                        {
                            printBoard(_game.Place(currentPlayer, x));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Invalid move. Try again.");
                        }

                        Console.WriteLine("");

                    }
                }

                if (_game.IsDraw())
                {
                    Console.WriteLine($"Draw!");
                }
                else
                {
                    Console.WriteLine($"Congratulations {_game.LastPlayer}!");
                    score[_game.LastPlayer] = score[_game.LastPlayer] + 1;
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
            //print column labels
            for (int col = 0; col < board.GetLength(1); col++)
            {
                Console.ForegroundColor = _boardColor;
                Console.Write($" {col + 1} ");
            }
            Console.WriteLine();

            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    Console.ForegroundColor = _boardColor;
                    Console.Write("[");

                    var occupant = board[row, col];
                    if (occupant == _player1) Console.ForegroundColor = _player1Color;
                    if (occupant == _player2) Console.ForegroundColor = _player2Color;
                    if (_game.WinCoords().Contains((row, col))) Console.ForegroundColor = _winColor;
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
}
