using TMPro;
using UnityEngine;

public class AgentDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text fitnessField;
    [SerializeField] private Agent agent;

    void Update()
    {
        fitnessField.text = $"Fitness: {agent.CalcFitness()}";
    }
}
