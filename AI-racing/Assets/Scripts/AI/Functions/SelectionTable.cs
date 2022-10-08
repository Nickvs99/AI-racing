using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SelectionTable
{
    // Each method takes an array of floats (fitnesses) and returns the selected indices based on the fitness
    public static Dictionary<string, Func<float[], int[]>> table = new Dictionary<string, Func<float[], int[]>>()
    {
        {"Default", Default},
        {"Softmax", Softmax },
        {"Tournament", Tournament2 }
    };

    /// <summary>
    /// Pick probability is proportional to their fitness differenceto the lowest fitness.
    /// </summary>
    /// <returns></returns>
    private static int[] Default(float[] fitnesses)
    {
        int populationSize = fitnesses.Length;
        int[] indices = new int[populationSize];

        // Cumulate fitness values
        float[] cumFitnesses = new float[populationSize];
        float cumFitness = 0;

        float worstFitness = fitnesses.Min();
        for (int i = 0; i < populationSize; i++)
        {
            // Worst fitness is substracted to allow negative fitness values. Also increases probability of higher
            // performing neural networks once all fitness values are high.
            cumFitness += fitnesses[i] - worstFitness;
            cumFitnesses[i] = cumFitness;
        }

        // Normalise fitness
        for (int i = 0; i < populationSize; i++)
        {
            cumFitnesses[i] /= cumFitness;
        }

        for (int i = 0; i < populationSize; i++)
        {
            indices[i] = GetRandomIndexFromSortedNormalizedArray(cumFitnesses);
        }

        return indices;
    }

    /// <summary>
    /// Pick probability is set through the softmax function
    /// </summary>
    /// <param name="fitnesses"></param>
    /// <returns></returns>
    private static int[] Softmax(float[] fitnesses)
    {
        int populationSize = fitnesses.Length;
        int[] indices = new int[populationSize];

        // Substract maxFitness to prevent overflow
        float maxFitness = fitnesses.Max();
        float[] relativeFitnesses = fitnesses.Select(fitness => fitness - maxFitness).ToArray();

        float totalFitness = 0;
        foreach(float fitness in relativeFitnesses)
        {
            totalFitness += Mathf.Exp(fitness);
        }

        float[] cumFitnesses = new float[populationSize];
        float cumFitness = 0;
        for(int i = 0; i < populationSize; i++)
        {
            cumFitness += Mathf.Exp(relativeFitnesses[i]) / totalFitness;
            cumFitnesses[i] = cumFitness;
        }

        for (int i = 0; i < populationSize; i++)
        {
            indices[i] = GetRandomIndexFromSortedNormalizedArray(cumFitnesses);
        }

        return indices;
    }

    private static int GetRandomIndexFromSortedNormalizedArray(float[] cumValues)
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        int index;
        for (index = 0; index < cumValues.Length; index++)
        {
            if (r < cumValues[index])
            {
                return index;
            }
        }

        Debug.LogError("No valid index was found. Could be due to a non sorted array or improper normalization.");
        return index - 1;
    }

    private static int[] Tournament(float[] fitnesses, int tournamentSize)
    {
        int populationSize = fitnesses.Length;
        int[] indices = new int[populationSize];

        for (int i = 0; i < populationSize; i++)
        {
            // Pick n random indices
            int[] tournamentIndices = SelectionSamplingIndices(populationSize, tournamentSize);
            
            // Get fitness value associated with those indices
            float[] tournamentFitnesses = new float[tournamentSize];
            for (int j = 0; j < tournamentIndices.Length; j++)
            {
                tournamentFitnesses[j] = fitnesses[tournamentIndices[j]];
            }

            // Select index with the max fitness in the tournament
            int maxIndex = Array.IndexOf(fitnesses, tournamentFitnesses.Max());
            indices[i] = maxIndex;
        }

        return indices;
    }

    /// <summary>
    /// Returns n random indices from N elements, 0 < n <= N, each with equal probability. No duplicates allowed.
    /// <see cref="https://stackoverflow.com/a/35065765"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="sampleSize"></param>
    /// <returns></returns>
    private static int[] SelectionSamplingIndices(int N, int sampleSize)
    {
        int[] chosenIndices = new int[sampleSize];
        int nChosen = 0;

        for(int i = 0; i < N; i++)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            float chosenProbability = ((float) sampleSize - nChosen) / (N - i);

            if (r < chosenProbability)
            {
                chosenIndices[nChosen] = i;
                nChosen += 1;

                if (nChosen == sampleSize)
                {
                    return chosenIndices;
                }
            }
        }

        Debug.LogError("Error: Something went wrong with Selection sampling.");
        return chosenIndices;
    }

    private static int[] Tournament2(float[] fitnesses)
    {
        return Tournament(fitnesses, 2);
    }
}
