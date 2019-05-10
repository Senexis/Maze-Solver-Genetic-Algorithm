using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeSolverGeneticAlgorithm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region Debug code

            //var a = maze.GetStartPositionColumn();
            //var b = maze.GetStartPositionRow();
            //var c = maze.GetEndPositionColumn();
            //var d = maze.GetEndPositionRow();
            //var e = maze.MoveIsWithinBounds(a, b, Maze.RIGHT_MOVE);
            //var ex = maze.MoveIsWithinBounds(a, b, Maze.LEFT_MOVE);
            //var f = maze.MoveIsValid(a, b, Maze.RIGHT_MOVE);
            //var fx = maze.MoveIsValid(a, b, Maze.LEFT_MOVE);
            //var g = new Individual(maze);
            //var h = g.CalculateFitness();

            //var gx = new List<Individual>();

            //for (int i = 0; i < 5000; i++)
            //{
            //    gx.Add(new Individual(maze));
            //}

            //foreach (var individual in gx)
            //{
            //    individual.CalculateFitness();
            //    individual.MutatePath();
            //}

            //var gy = gx.Max(individual => individual.CalculateFitness());

            //var simpleMaze = new Maze(new List<string>()
            //{
            //    "xxxxxxxx",
            //    "xS    Ex",
            //    "xxxxxxxx"
            //});

            //var impossibleMaze = new Maze(new List<string>()
            //{
            //    "xxxxxxxx",
            //    "xS xx Ex",
            //    "xxxxxxxx"
            //});

            #endregion

            var maze = new Maze(new List<string>
            {
                "xxxxxxxxxxx",
                "x        Sx",
                "xxxxxxx xxx",
                "x         x",
                "x xxxx x xx",
                "x x    x  x",
                "xxx xxxxx x",
                "x     xx  x",
                "x xxxxx  xx",
                "x  Ex   xxx",
                "xxxxxxxxxxx"
            });

            var winners = new Dictionary<Individual, int>();
            var generation = new Generation(maze, new Random(), 1000, 2500);

            while (generation.IsBreedable())
            {
                foreach (var individual in generation.GetPopulation())
                {
                    individual.CalculateFitness();

                    if (maze.GetTileAtPosition(individual.GetColPosition(), individual.GetRowPostion()) == Maze.EndTile)
                    {
                        winners.Add(individual, generation.GetEvolutionCount() + 1);
                        Console.WriteLine($"Winner found in generation {generation.GetEvolutionCount() + 1}!");

                        // TODO: Instead of setting this to decrease by 1 every winner, use the shortest path somehow in dynamic learning.
                        // This will take me a bunch of research and will likely not be doable in the amount of time I have.
                        generation.SetPathLength(generation.GetPathLength() - 1);

                        // Only return the first winner, fixes the issue with the length decreasing more than once with more than one winner.
                        break;
                    }
                }

                var fitness = generation.GetFitnessScores();
                var weights = generation.NormalizeWeights(fitness);

                generation.Breed(weights);

                Console.WriteLine($"=== GENERATION {generation.GetEvolutionCount() + 1} ===");

                foreach (var individual in generation.GetOffspring())
                {
                    individual.MutatePath();
                    individual.CalculateFitness();
                }

                generation.Evolve();

                var max = generation.GetPopulation().Max(individual => individual.GetFitness());
                var best = generation.GetPopulation().Find(individual => individual.GetFitness() == max);

                Console.WriteLine($"Best individual: {best.GetFitness()}, length: {best.GetPath().Length}, path: {best.GetPath()}, X: {best.GetRowPostion()}, Y: {best.GetColPosition()}");
            }

            var winnerString = "";
            foreach (var winner in winners)
            {
                winnerString += $"- Gen {winner.Value}, fitness {winner.Key.GetFitness()}, length {winner.Key.GetPath().Length}, path {winner.Key.GetPath()}\n";
            }

            if (winners.Count > 0)
            {
                var bestWinner = winners.Reverse().First();
                var bestString = $"Best run:\nGen {bestWinner.Value}, fitness {bestWinner.Key.GetFitness()}, length {bestWinner.Key.GetPath().Length}, path {bestWinner.Key.GetPath()}\n";

                Console.WriteLine($"{bestString}\n{winners.Count} total winner(s):\n{winnerString}");
                MessageBox.Show($"{bestString}\n{winners.Count} total winner(s):\n{winnerString}");
            }
            else
            {
                MessageBox.Show("There were no winners today. :-(");
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
