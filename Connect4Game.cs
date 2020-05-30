using System;
using System.Collections.Generic;
using System.Text;

namespace Connect4
{
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
