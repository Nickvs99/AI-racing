using System;
using System.Collections.Generic;

public static class TerminationTable
{
    // Each method takes the avgs and max fitnesses per generation and determines if the evolution should be terminated.
    public static Dictionary<string, Func<List<float>, List<float>, bool>> table = new Dictionary<string, Func<List<float>, List<float>, bool>>()
    {
        {"Default", Endless },
        {"Endless", Endless },
        {"Fixed", Fixed },
        {"Short", Short },
        {"Long", Long },
        {"Research", Research }
    };

    private static bool Endless(List<float> avgs, List<float> maxs)
    {
        return false;
    }

    private static bool Fixed(List<float> avgs, List<float> maxs)
    {
        return avgs.Count >= 25;
    }

    private static bool Short(List<float> avgs, List<float> maxs)
    {
        return avgs.Count >= 5;
    }

    private static bool Long(List<float> avgs, List<float> maxs)
    {
        return avgs.Count >= 1000;
    }

    private static bool Research(List<float> avgs, List<float> maxs)
    {
        // Current avg value should be greater than the avg value N generations ago.
        // Read more about it in the termination.ipynb notebook
        int N = 50;

        return avgs.Count > N && avgs[avgs.Count - 1] < avgs[avgs.Count - 1 - N];
    }
}
