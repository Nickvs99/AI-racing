using System;
using System.Collections.Generic;
using UnityEngine;

public static class MutationTable
{
    public static Dictionary<string, Func<float, float, float>> weightTable = new Dictionary<string, Func<float, float, float>>()
    {
        {"Default", WeightDefault},
        {"NormalSigmaScaled", NormalSigmaScaledMutation},
    };

    public static Dictionary<string, Func<float, float, float>> biasTable = new Dictionary<string, Func<float, float, float>>()
    {
        {"Default", BiasDefault},
        {"NormalSigmaScaled", NormalSigmaScaledMutation},
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

    private static float NormalSigmaScaledMutation(float value, float mutateProbability)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < mutateProbability)
        {
            mutation = NormalSigmaScaled(value);
        }

        return value + mutation;
    }

    public static float GenerateNormalValue(float mu, float sigma)
    {
        float r1 = UnityEngine.Random.Range(0.0f, 1.0f);
        float r2 = UnityEngine.Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt(-2f * Mathf.Log(r1)) * Mathf.Cos((2f * Mathf.PI) * r2);

        return (mu + sigma * n);
    }

    public static float NormalSigmaScaled(float mu)
    {
        // Scale the sigma depending on the mu, have a minimum mu value such that 
        // their is still variance when mu is close to 0
        float sigma = Math.Max(0.2f * mu, 0.04f);
        
        return GenerateNormalValue(mu, sigma);
    }
}
