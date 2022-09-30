using System;
using System.Collections.Generic;

public static class ActivationTable
{
    // Maps a string to a function. This is done since it allows the string to be saved. When loaded it can
    // then retrieve the corresponding function.
    public static Dictionary<string, Func<float, float>> table = new Dictionary<string, Func<float, float>>()
    {
        {"Default", Default},
        {"Sigmoid", Sigmoid},
    };

    private static float Default(float x)
    {
        return x;
    }

    private static float Sigmoid(float x)
    {
        return 1f / (1f + (float)Math.Exp(-x));
    }
}