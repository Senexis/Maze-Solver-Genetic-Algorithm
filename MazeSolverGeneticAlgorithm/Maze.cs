using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolverGeneticAlgorithm
{
    class Maze
    {
        /// <summary>
        /// A list of valid moves to make within the maze.
        /// </summary>
        public static readonly char[] ValidMoves = { 'U', 'D', 'L', 'R' };

        /// <summary>
        /// The character corresponding to moving up.
        /// </summary>
        public const char UpMove = 'U';

        /// <summary>
        /// The character corresponding to moving down.
        /// </summary>
        public const char DownMove = 'D';

        /// <summary>
        /// The character corresponding to moving left.
        /// </summary>
        public const char LeftMove = 'L';

        /// <summary>
        /// The character corresponding to moving right.
        /// </summary>
        public const char RightMove = 'R';

        /// <summary>
        /// The character corresponding to the start tile on which the individual starts a maze.
        /// </summary>
        public const char StartTile = 'S';

        /// <summary>
        /// The character corresponding to the end tile on which the individual finishes a maze.
        /// </summary>
        public const char EndTile = 'E';

        /// <summary>
        /// The character corresponding to an open tile on which the individual can walk.
        /// </summary>
        public const char OpenTile = ' ';

        /// <summary>
        /// The character corresponding to a wall tile which blocks the individual from moving.
        /// </summary>
        public const char WallTile = 'x';

        /// <summary>
        /// The internal set of tiles formatted by row.
        /// </summary>
        private readonly List<string> _tiles;

        /// <summary>
        /// The internal counter of total tiles in a maze.
        /// </summary>
        private readonly int _tilesLength;

        /// <summary>
        /// Create a new maze instance using the tiles list provided.
        /// </summary>
        public Maze(List<string> tiles)
        {
            _tiles = tiles;

            foreach (var row in tiles)
            {
                _tilesLength += row.Length;
            }
        }

        /// <summary>
        /// Gets the row position of the start tile.
        /// </summary>
        public int GetStartPositionRow()
        {
            for (var index = 0; index < _tiles.Count; index++)
            {
                if (_tiles[index].Contains(StartTile)) {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the column position of the start tile.
        /// </summary>
        public int GetStartPositionColumn()
        {
            var row = GetStartPositionRow();

            for (var index = 0; index < _tiles[row].Length; index++)
            {
                if (_tiles[row].ElementAt(index) == StartTile)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the row position of the end tile.
        /// </summary>
        public int GetEndPositionRow()
        {
            for (var index = 0; index < _tiles.Count; index++)
            {
                if (_tiles[index].Contains(EndTile))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the column position of the end tile.
        /// </summary>
        public int GetEndPositionColumn()
        {
            var row = GetEndPositionRow();

            for (var index = 0; index < _tiles[row].Length; index++)
            {
                if (_tiles[row].ElementAt(index) == EndTile)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the tiles in the maze.
        /// </summary>
        public List<string> GetTiles()
        {
            return _tiles;
        }

        /// <summary>
        /// Gets the total amount of tiles in the maze.
        /// </summary>
        public int GetTilesCount()
        {
            return _tilesLength;
        }

        /// <summary>
        /// Gets the tile at the provided position.
        /// </summary>
        public char GetTileAtPosition(int col, int row)
        {
            return _tiles[row][col];
        }

        /// <summary>
        /// Checks whether the move would move the individual outside of the boundaries of the maze.
        /// </summary>
        public bool MoveIsWithinBounds(int col, int row, char move)
        {
            return
                (move == UpMove && row != _tiles.Count - 1) ||
                (move == DownMove && row != 0) ||
                (move == LeftMove && col != 0) ||
                (move == RightMove && col != _tiles[row][col - 1]);
        }

        /// <summary>
        /// Checks whether the move would hit a wall tile if it were to be executed.
        /// </summary>
        public bool MoveIsValid(int col, int row, char move)
        {
            return
                (move == UpMove && _tiles[row + 1][col] != WallTile) ||
                (move == DownMove && _tiles[row - 1][col] != WallTile) ||
                (move == LeftMove && _tiles[row][col - 1] != WallTile) ||
                (move == RightMove && _tiles[row][col + 1] != WallTile);
        }

        public override string ToString()
        {
            var result = "";

            foreach (var row in _tiles)
            {
                result += row + Environment.NewLine;
            }

            return result;
        }
    }
}
