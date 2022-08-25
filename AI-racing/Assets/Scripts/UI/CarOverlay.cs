using UnityEngine;

public class CarOverlay : MonoBehaviour
{

    [SerializeField] private CarManager carManager;
    [SerializeField] private Speedometer speedometer;

    void Update()
    {
        speedometer.SetDisplay(carManager);
    }
}
