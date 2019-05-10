using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeSolverGeneticAlgorithm
{
    class Generation
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
        /// The internal instance of the maximum number of evolutions.
        /// </summary>
        private readonly int _maxEvolutionCount;

        /// <summary>
        /// The internal instance of the current evolution number.
        /// </summary>
        private int _currentEvolutionCount;

        /// <summary>
        /// The internal instance of a list of individuals considered the current population.
        /// </summary>
        private List<Individual> _population = new List<Individual>();

        /// <summary>
        /// The internal instance of a list of individuals considered the current offspring.
        /// </summary>
        private List<Individual> _offspring = new List<Individual>();

        /// <summary>
        /// Create a new instance of a generation which will fill its population based off the provided population count.
        /// </summary>
        public Generation(Maze maze, Random random, int population = 100, int generations = 5000)
        {
            _maze = maze;
            _maxEvolutionCount = generations;
            _random = random;

            for (var i = 0; i < population; i++)
            {
                _population.Add(new Individual(maze, random));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Breeds the current population and adds the children to the offspring list.
        /// Logic:
        /// 1. Select two parents.
        /// 2. Generate a random midway number of which to base the new paths using genome splitting.
        /// 3. Create two new paths, one of parent one as a primary basis, and the other of parent two as a primary basis.
        /// 4. Add both children to the offspring list.
        /// </summary>
        public List<Individual> Breed(List<double> weights)
        {
            for (var i = 0; i < Math.Floor((decimal) (_population.Count / 2)); i++)
            {
                var indexParentOne = Selection(weights);
                var indexParentTwo = Selection(weights);

                var splitPoint = _random.Next(1, _maze.GetTilesCount() - 1);

                var pathOne = _population[indexParentOne].GetPath().Substring(0, splitPoint) +
                              _population[indexParentTwo].GetPath().Substring(splitPoint, _maze.GetTilesCount() - splitPoint);
                var pathTwo = _population[indexParentTwo].GetPath().Substring(0, splitPoint) +
                              _population[indexParentOne].GetPath().Substring(splitPoint, _maze.GetTilesCount() - splitPoint);

                var childOne = new Individual(_maze, _random, pathOne);
                var childTwo = new Individual(_maze, _random, pathTwo);

                _offspring.Add(childOne);
                _offspring.Add(childTwo);
            }

            _currentEvolutionCount++;
            return _offspring;
        }

        /// <summary>
        /// Evolve the current generation by adding all of the offspring to the population list, effectively killing off the original population.
        /// </summary>
        public List<Individual> Evolve()
        {
            _population = _offspring;
            _offspring = new List<Individual>();
            return _population;
        }

        /// <summary>
        /// Runs the entire generation using all of the methods and logic found elsewhere.
        /// </summary>
        public void Run()
        {
            while (IsBreedable())
            {
                foreach (var individual in GetPopulation())
                {
                    individual.CalculateFitness();

                    if (_maze.GetTileAtPosition(individual.GetColPosition(), individual.GetRowPostion()) == Maze.EndTile)
                    {
                        Console.WriteLine($"Winner found at generation {GetEvolutionCount() + 1}!");
                        return;
                    }
                }

                var fitness = GetFitnessScores();
                var weights = NormalizeWeights(fitness);

                Breed(weights);

                Console.WriteLine($"=== GENERATION {GetEvolutionCount() + 1} ===");

                foreach (var individual in GetOffspring())
                {
                    individual.MutatePath();
                    individual.CalculateFitness();
                }

                Evolve();

                var max = GetPopulation().Max(individual => individual.GetFitness());
                var best = GetPopulation().Find(individual => individual.GetFitness() == max);

                Console.WriteLine($"Best individual: {best.GetFitness()}, path: {best.GetPath()}, X: {best.GetRowPostion()}, Y: {best.GetColPosition()}");
            }
        }

        /// <summary>
        /// Assesses if the current generation is able to be bred by looking at the evolution count and comparing it with the maximal amount of evolutions.
        /// </summary>
        /// <returns></returns>
        public bool IsBreedable()
        {
            return _currentEvolutionCount < _maxEvolutionCount;
        }

        /// <summary>
        /// Gets the current list of individuals in the population.
        /// </summary>
        public List<Individual> GetPopulation()
        {
            return _population;
        }

        /// <summary>
        /// Gets the current list of individuals in the offspring.
        /// </summary>
        public List<Individual> GetOffspring()
        {
            return _offspring;
        }

        /// <summary>
        /// Gets the current number of evolutions successfully put onto the generation.
        /// </summary>
        public int GetEvolutionCount()
        {
            return _currentEvolutionCount;
        }

        /// <summary>
        /// Gets a list of fitness scores the current population has.
        /// </summary>
        /// <returns></returns>
        public List<double> GetFitnessScores()
        {
            return _population.Select(individual => individual.GetFitness()).ToList();
        }

        /// <summary>
        /// Selects a random list index using the Monte Carlo selection method.
        /// Logic:
        /// 1. Pick a random number between the highest weight and 0.
        /// 2. Loop through the weights and if the current iteration is better than the random number, pick that one.
        /// This is a greedy search method, which just hopes to pick the best one.
        /// </summary>
        public int Selection(List<double> weights)
        {
            var index = _random.Next(0, (int) Math.Ceiling(weights[weights.Count - 1]));

            for (var i = 0; i < weights.Count; i++)
            {
                var weight = weights[i];

                if (index <= weight)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Creates a linear, normalized set of weights.
        /// </summary>
        public List<double> NormalizeWeights(List<double> weights)
        {
            var values = new List<double>();
            var results = new List<double>();
            var accumulativeWeight = 0.0;

            foreach (var weight in weights)
            {
                values.Add(weight / weights.Sum(d => d) * 100 + 0.5);
            }

            foreach (var value in values)
            {
                accumulativeWeight += value;
                results.Add(accumulativeWeight);
            }

            return results;
        }
    }
}
