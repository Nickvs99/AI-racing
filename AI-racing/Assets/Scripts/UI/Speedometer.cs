using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TMP_Text speedField;

    public void SetDisplay(CarManager carManager)
    {
        speedField.text = Mathf.RoundToInt(carManager.speed).ToString();
    }
}
