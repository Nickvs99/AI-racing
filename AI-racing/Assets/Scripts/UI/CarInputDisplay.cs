using UnityEngine;
using UnityEngine.UI;

public class CarInputDisplay : MonoBehaviour
{
    [SerializeField] private Image arrow;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = arrow.GetComponent<RectTransform>();
    }

    public void SetDisplay(CarManager carManager)
    {
        Vector2 input = carManager.carInput;

        float yScale = input.magnitude;

        // Calculate the angle against the up vector. This is because a 0 rotation points straight up.
        float angle = Vector2.SignedAngle(Vector2.up, input);

        rectTransform.localScale = new Vector3(rectTransform.localScale.x, yScale, rectTransform.localScale.z);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
