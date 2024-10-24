using System;
using Tuples;

namespace Abyss.Environment
{
    public class GA
    {
        private readonly Random random = new();
        public int ChooseWithProbability(double[] probabilities)
        {
            double randomValue = random.NextDouble();

            double cumulativeProbability = 0.0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue < cumulativeProbability)
                    return i;
            }

            throw new InvalidOperationException("The probability distribution should sum to 1.");
        }

        public int[] SelectParent(int populationSize)
        {
            int[] parent = new int[2];

            parent[0] = random.Next(0, populationSize);
            parent[1] = random.Next(0, populationSize);
            while ((parent[0] == parent[1]) && (populationSize > 1))
                parent[1] = random.Next(0, populationSize);
            return parent;
        }

        public float Mutate(float x, float y, float mutationRate, float rangeMin, float rangeMax)
        {
            double xRate = (1 - mutationRate) / 2;
            double yRate = xRate;
            double[] probabilities = { xRate, yRate, mutationRate };
            int chosenNumber = ChooseWithProbability(probabilities);
            if (chosenNumber == 0) return x;
            if (chosenNumber == 1) return y;
            return (float)(rangeMin + (random.NextDouble() * (rangeMax - rangeMin)));
        }

        public float[,] GetInitPopulation(int populationSize, Pair<float, float>[] ranges, int chromosomeLength)
        {
            float[,] population = new float[populationSize, chromosomeLength];

            for (int i = 0; i < populationSize; i++)
            {
                for (int j = 0; j < chromosomeLength; j++)
                {
                    float randonNum = (float)(ranges[j].Head + (random.NextDouble() * (ranges[j].Tail - ranges[j].Head)));
                    float midRange = (ranges[j].Tail + ranges[j].Head) / 2.0f;
                    // Scale Size = 0.2
                    population[i, j] = (randonNum - midRange ) * 0.2f + midRange;
                }
            }

            return population;
        }

        public float[,] GetChildren(float[,] parents, float mutationRate, Pair<float, float>[] ranges, float generationRate = 2)
        {
            int populationSize = parents.GetLength(0);
            int chromosomeLength = parents.GetLength(1);
            int generationSize = (int)Math.Round(generationRate * (float)populationSize);
            float[,] children = new float[generationSize, chromosomeLength];

            for (int i = 0; i < generationSize; i++)
            {
                var selectedParents = SelectParent(populationSize);

                for (int j = 0; j < chromosomeLength; j++)
                {
                    float x = parents[selectedParents[0], j];
                    float y = parents[selectedParents[1], j];
                    children[i, j] = Mutate(x, y, mutationRate, ranges[j].Head, ranges[j].Tail);
                }
            }

            return children;
        }
    }
}