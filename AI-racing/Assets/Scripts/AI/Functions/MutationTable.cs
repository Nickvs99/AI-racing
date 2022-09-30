using System;
using System.Collections.Generic;
using UnityEngine;

public static class MutationTable
{
    public static Dictionary<string, Func<float, float, float>> weightTable = new Dictionary<string, Func<float, float, float>>()
    {
        {"Default", WeightDefault},
    };

    public static Dictionary<string, Func<float, float, float>> biasTable = new Dictionary<string, Func<float, float, float>>()
    {
        {"Default", BiasDefault},
    };

    private static float WeightDefault(float weight, float mutateProbability)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < mutateProbability)
        {
            mutation = UnityEngine.Random.Range(-0.5f, 0.5f);
        }

        return weight + mutation;
    }

    private static float BiasDefault(float bias, float mutateProbability)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < mutateProbability)
        {
            mutation = UnityEngine.Random.Range(-0.2f, 0.2f);
        }
        return bias + mutation;
    }
}
