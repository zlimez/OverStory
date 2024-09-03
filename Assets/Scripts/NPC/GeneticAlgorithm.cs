using System;
using UnityEngine;
public class GA
{
    // int populationSize;
    // int generationSize;
    // int chromosomeLength;
    float rangeMin = 0;
    float rangeMax = 10;
    // float generationRate = 2;
    float mutationRate = (float)0.1;
    private System.Random random = new();
    public int ChooseWithProbability(double[] probabilities)
    {
        double randomValue = random.NextDouble();
        
        double cumulativeProbability = 0.0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability)
            {
                return i;
            }
        }

        throw new InvalidOperationException("The probability distribution should sum to 1.");
    }

    public int[] SelectParent(int populationSize)
    {
        int[] parent = new int[2];

        parent[0] = random.Next(0, populationSize);
        parent[1] = random.Next(0, populationSize);
        while ((parent[0] == parent[1])&&(populationSize>1)) 
            parent[1] = random.Next(0, populationSize);
        return parent;
    }

    public float Mutate(float x, float y)
    {
        double xRate = (1-mutationRate)/2;
        double yRate = xRate;
        double[] probabilities = {xRate, yRate, mutationRate};
        int chosenNumber = ChooseWithProbability(probabilities);
        if(chosenNumber == 0) return x;
        if(chosenNumber == 1) return y;
        return (float)(rangeMin + (random.NextDouble() * (rangeMax - rangeMin)));
    }
    
    public float[,] GetGenerations(float[,] parents, float generationRate = 2)
    {
        int populationSize = parents.GetLength(0);
        int chromosomeLength = parents.GetLength(1);
        int generationSize = (int)Math.Round(generationRate * (float)populationSize);
        float[,] generations = new float[generationSize, chromosomeLength];

        int[] selectedParents = new int[2];
        for (int i = 0; i < generationSize; i++)
        {
            selectedParents = SelectParent(populationSize);

            for(int j = 0; j < chromosomeLength; j++)
            {
                float x = parents[selectedParents[0], j];
                float y = parents[selectedParents[1], j];
                generations[i, j] = Mutate(x, y);
            }
        }

        return generations;
    }
}