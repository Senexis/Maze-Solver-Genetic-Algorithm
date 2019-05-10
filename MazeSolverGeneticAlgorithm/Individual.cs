using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolverGeneticAlgorithm
{
    class Individual
    {
        /// <summary>
        /// The internal instance of the maze associated with an individual.
        /// </summary>
        private readonly Maze _maze;

        /// <summary>
        /// The internal instance of a random number generation unit.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// The internal instance of the full path generated for an individual.
        /// </summary>
        private string _path = "";

        /// <summary>
        /// The internal instance of the current row position of an individual.
        /// </summary>
        private int _rowPos = -1;

        /// <summary>
        /// The internal instance of the current column position of an individual.
        /// </summary>
        private int _colPos = -1;

        /// <summary>
        /// The internal instance of the current fitness score of an individual.
        /// The higher the fitness score, the better the performance of an individual.
        /// </summary>
        private double _fitness = -1;

        /// <summary>
        /// The internal instance of the current distance between the individual and the end point of the maze.
        /// </summary>
        private double _endDistance = -1;

        /// <summary>
        /// Create a new instance of an individual which will generate its own path randomly from a list of valid maze moves.
        /// </summary>
        public Individual(Maze maze, Random random)
        {
            _maze = maze;
            _random = random;

            while (_path.Length < maze.GetTilesCount())
            {
                var index = random.Next(0, 4);
                _path += Maze.ValidMoves[index];
            }
        }

        /// <summary>
        /// Create a new instance of an individual without generating its own path randomly.
        /// </summary>
        public Individual(Maze maze, Random random, string path)
        {
            _maze = maze;
            _random = random;
            _path = path;
        }

        /// <summary>
        /// Calculates the fitness of the individual using the following logic:
        /// 1. Check if the move would put the individual out of bounds. If so, add one point to the blocked counter.
        /// 2. Check if the move would put the individual in a wall. If so, add one point to the blocked counter.
        /// 3. Check if the move isn't a repeat of the previous or next move. If so, add one point to the non-loop counter.
        /// 4. Check if the move isn't a backtracking move (up -> down -> up or right -> left -> right). If so, add one point to the non-loop counter.
        /// 5. Add a small bonus point for each step moved.
        /// 6. Add a bonus for ending closest to the end point.
        /// </summary>
        public double CalculateFitness()
        {
            _rowPos = _maze.GetStartPositionRow();
            _colPos = _maze.GetStartPositionColumn();

            var rowPosEnd = _maze.GetEndPositionRow();
            var colPosEnd = _maze.GetEndPositionColumn();

            var efficiency = 0;
            var blockedCount = 0;
            var traveled = 0;
            var noLoop = 0;

            var moves = Maze.ValidMoves;

            for (var index = 0; index < _path.Length; index++)
            {
                var move = _path[index];

                if (!_maze.MoveIsWithinBounds(_colPos, _rowPos, move))
                {
                    blockedCount += 1;
                    continue;
                }

                if (!_maze.MoveIsValid(_colPos, _rowPos, move))
                {
                    blockedCount += 1;
                    continue;
                }

                if (index - 1 >= 0 && index + 1 < _path.Length)
                {
                    var prevMove = _path[index - 1];
                    var nextMove = _path[index + 1];
                    if (
                        move != prevMove && move != nextMove &&
                        (move == Maze.UpMove && (prevMove != Maze.DownMove && nextMove != Maze.DownMove)) &&
                        (move == Maze.DownMove && (prevMove != Maze.UpMove && nextMove != Maze.UpMove)) &&
                        (move == Maze.LeftMove && (prevMove != Maze.RightMove && nextMove != Maze.RightMove)) &&
                        (move == Maze.RightMove && (prevMove != Maze.LeftMove && nextMove != Maze.LeftMove))
                    )
                    {
                        noLoop += 1;
                    }
                }

                // TODO: Split to method.
                if (move == Maze.UpMove && _rowPos != (_maze.GetTiles().Count - 1))
                {
                    _rowPos += 1;
                    traveled += 1;
                }

                if (move == Maze.DownMove && _rowPos != 0)
                {
                    _rowPos -= 1;
                    traveled += 1;
                }

                if (move == Maze.RightMove && _colPos != (_maze.GetTiles()[0].Length - 1))
                {
                    _colPos += 1;
                    traveled += 1;
                }

                if (move == Maze.LeftMove && _colPos != 0)
                {
                    _colPos -= 1;
                    traveled += 1;
                }
            }

            // TODO: Implement efficiency by making path length dynamic to encourage shortest paths.
            efficiency = _maze.GetTilesCount() - _path.Length;

            _fitness = (traveled * 0.01) + (efficiency * 2) + noLoop - blockedCount;

            _endDistance = Math.Sqrt(Math.Pow(colPosEnd - _colPos, 2) + Math.Pow(rowPosEnd - _rowPos, 2));
            _fitness += (_maze.GetTilesCount() - _endDistance);

            if (_maze.GetTileAtPosition(_colPos, _rowPos) == Maze.EndTile)
                _fitness += _path.Length;

            if (_fitness < 0)
                _fitness = -1;

            return _fitness;
        }

        /// <summary>
        /// 60% of the time, randomly mutate a part of the path while keeping other parts intact.
        /// Logic:
        /// 1. Get a snippet in the middle of the path string using a random snippet length.
        /// 2. Shuffle the characters in the snippet.
        /// 3. Replace the characters of the original snippet's location with the snippet.
        /// </summary>
        public string MutatePath()
        {
            if (!(_random.NextDouble() <= 0.6)) return _path;

            var lowerPath = _random.Next(1, _path.Length - 2);
            var upperPath = _random.Next(lowerPath + 1, _path.Length);
            var snippetLength = upperPath - lowerPath;

            var snippet = _path.Substring(lowerPath, snippetLength);
            var snippetShuffled = new string(snippet.ToCharArray().OrderBy(s => (_random.Next(2) % 2) == 0).ToArray());

            _path = _path.Substring(0, lowerPath) + snippetShuffled + _path.Substring(upperPath, _path.Length - upperPath);

            return _path;
        }

        /// <summary>
        /// Gets the current column position of the individual.
        /// </summary>
        public int GetColPosition()
        {
            return _colPos;
        }

        /// <summary>
        /// Gets the current row position of the individual.
        /// </summary>
        public int GetRowPostion()
        {
            return _rowPos;
        }

        /// <summary>
        /// Gets the current path associated with the individual.
        /// </summary>
        public string GetPath()
        {
            return _path;
        }

        /// <summary>
        /// Gets a rounded variant of the current individual's fitness score.
        /// </summary>
        public double GetFitness()
        {
            return Math.Round(_fitness, 1);
        }

        /// <summary>
        /// Get the distance between the individual and the end tile.
        /// </summary>
        public double GetEndDistance()
        {
            return _endDistance;
        }

        public override string ToString()
        {
            return $"An individual with fitness level {GetFitness()}.";
        }
    }
}
