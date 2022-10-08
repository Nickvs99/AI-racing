using System;
using System.Collections.Generic;

public static class TerminationTable
{
    // Each method takes the avgs and max fitnesses per generation and determines if the evolution should be terminated.
    public static Dictionary<string, Func<List<float>, List<float>, bool>> table = new Dictionary<string, Func<List<float>, List<float>, bool>>()
    {
        {"Default", Endless },
        {"Endless", Endless }

    };

    private static bool Endless(List<float> avgs, List<float> maxs)
    {
        return false;
    }
}
