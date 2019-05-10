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

            var generation = new Generation(maze, new Random());
            generation.Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
