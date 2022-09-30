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
        {"SigmoidScaled", SigmoidScaled },
        {"SigmoidScaledSharp", SigmoidScaledSharp },
        {"SigmoidScaledSmooth", SigmoidScaledSmooth },
    };

    private static float Default(float x)
    {
        return x;
    }

    private static float Sigmoid(float x)
    {
        return 1f / (1f + (float)Math.Exp(-x));
    }

    // Sigmoid function but with range -1 to 1
    private static float SigmoidScaled(float x)
    {
        return 2f * Sigmoid(x) - 1f;
    }

    // Interestingly, this is the same as tanh
    private static float SigmoidScaledSharp(float x)
    {
        return SigmoidScaled(2f * x);
    }

    private static float SigmoidScaledSmooth(float x)
    {
        return SigmoidScaled(0.5f * x);
    }
}
