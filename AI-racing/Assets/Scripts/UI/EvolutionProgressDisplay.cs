using TMPro;
using UnityEngine;

public class EvolutionProgressDisplay : MonoBehaviour
{
    [SerializeField] private EvolutionManager manager;

    [SerializeField] private TMP_Text generationProgressField;
    [SerializeField] private TMP_Text previousGenerationField;
    [SerializeField] private TMP_Text overallGenerationField;

    public void UpdateGenerationProgressField(int generation, int agentIndex)
    {
        generationProgressField.text = $"Generation: {generation} ({agentIndex})";
    }

    public void UpdatePreviousGenerationField(float best, float worst, float avg)
    {
        previousGenerationField.text = $"Previous\t{BestWorstAvgString(best, worst, avg)}";
    }

    public void UpdateOverallField(float best, float worst, float avg)
    {
        overallGenerationField.text = $"Overall\t{BestWorstAvgString(best, worst, avg)}";
    }

    private string BestWorstAvgString(float best, float worst, float avg)
    {
        string b = string.Format("{0:0.00}", best);
        string w = string.Format("{0:0.00}", worst);
        string a = string.Format("{0:0.00}", avg);

        return $"{b}\t{a}\t{w}";
    }
}
