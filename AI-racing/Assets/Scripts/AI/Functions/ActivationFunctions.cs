using System;

public static class ActivationFunctions 
{
    public static float Default(float x)
    {
        return x;
    }

    public static float Sigmoid(float x)
    {
        return 1f / (1f + (float)Math.Exp(-x));
    }
}
