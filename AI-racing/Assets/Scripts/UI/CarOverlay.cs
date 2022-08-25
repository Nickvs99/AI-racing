using UnityEngine;

public class CarOverlay : MonoBehaviour
{

    [SerializeField] private CarManager carManager;
    [SerializeField] private Speedometer speedometer;
    [SerializeField] private CarInputDisplay carInputDisplay;

    void Update()
    {
        speedometer.SetDisplay(carManager);
        carInputDisplay.SetDisplay(carManager);
    }
}
