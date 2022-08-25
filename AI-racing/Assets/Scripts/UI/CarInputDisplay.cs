using UnityEngine;
using UnityEngine.UI;

public class CarInputDisplay : MonoBehaviour
{
    [SerializeField] private Image steeringInputElement;
    [SerializeField] private Image engineInputElement;
    [SerializeField] private Image brakeInputElement;

    private RectTransform steeringRectTransform;
    private RectTransform engineRectTransform;
    private RectTransform brakeRectTransform;


    private void Awake()
    {
        steeringRectTransform = steeringInputElement.GetComponent<RectTransform>();
        engineRectTransform = engineInputElement.GetComponent<RectTransform>();
        brakeRectTransform = brakeInputElement.GetComponent<RectTransform>();

    }

    public void SetDisplay(CarInput carInput)
    {
        UpdateLocalScaleX(steeringRectTransform, carInput.steerInput);
        UpdateLocalScaleY(engineRectTransform, Mathf.Abs(carInput.engineInput));
        UpdateLocalScaleY(brakeRectTransform, carInput.brakeInput);

        UpdateColor(engineInputElement, carInput.engineInput);
    }

    public void UpdateLocalScaleX(RectTransform element, float value)
    {
        element.localScale = new Vector3(
            value,
            element.localScale.y,
            element.localScale.z
        );
    }

    public void UpdateLocalScaleY(RectTransform element, float value)
    {
        element.localScale = new Vector3(
            element.localScale.x,
            value,
            element.localScale.z
        );
    }

    public void UpdateColor(Image image, float value)
    {
        // TODO colors shouldn't be hardcoded
        if (value >= 0)
        {
            image.color = new Color32(254, 186, 50, 255);
        }
        else
        {
            image.color = new Color32(42, 43, 44, 255);
        }
    }
}
