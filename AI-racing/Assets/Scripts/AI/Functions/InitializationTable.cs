using System;
using System.Collections.Generic;
using UnityEngine;

public static class InitializationTable
{
    public static Dictionary<string, Func<float>> weightTable = new Dictionary<string, Func<float>>()
    {
        {"Default", WeightDefault},
    };

    public static Dictionary<string, Func<float>> biasTable = new Dictionary<string, Func<float>>()
    {
        {"Default", BiasDefault},
    };

    private static float WeightDefault()
    {
        return UnityEngine.Random.Range(-1f, 1f);
    }

    private static float BiasDefault()
    {
        return 0f;
    }
}
