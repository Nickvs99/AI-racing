using System;
using System.Collections.Generic;
using System.Linq;

public static class SelectionTable
{
    // Each method takes an array of floats (fitnesses) and returns the selected indices based on the fitness
    public static Dictionary<string, Func<float[], int[]>> table = new Dictionary<string, Func<float[], int[]>>()
    {
        {"Default", Default},
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
            float r = UnityEngine.Random.Range(0f, 1f);

            // Pick a neural network index based on the normalised fitness
            int index;
            for (index = 0; index < populationSize - 1; index++)
            {
                if (r < cumFitnesses[index])
                {
                    break;
                }
            }

            indices[i] = index;
        }

        return indices;
    }
}
