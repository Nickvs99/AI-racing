using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Engine)), RequireComponent(typeof(Brakes)), RequireComponent(typeof(SteeringWheel))]
public class CarManager : MonoBehaviour
{
    public Vector2 carInput { get; set; }

    private Engine engine;
    private Brakes brakes;
    private SteeringWheel steeringWheel;
    private Wheel[] wheels;

    // TEMP, will be combined in the carInput 
    public bool isBreaking;

    private void Awake()
    {
        wheels = GetComponentsInChildren<Wheel>();

        engine = GetComponent<Engine>();
        brakes = GetComponent<Brakes>();
        steeringWheel = GetComponent<SteeringWheel>();
    }

    private void FixedUpdate()
    {
        engine.ApplyForce(carInput.y, wheels);
        brakes.ApplyForce(isBreaking ? 1 : 0, wheels);
        steeringWheel.ApplySteeringAngle(carInput.x, wheels);
    }
}
