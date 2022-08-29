using TMPro;
using UnityEngine;

public class LapTrackerDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text lapTrackerField;
    [SerializeField] private LapTracker lapTracker;

    void Update()
    {
        lapTrackerField.text = $"Lap: {lapTracker.currentLap}";
    }
}
