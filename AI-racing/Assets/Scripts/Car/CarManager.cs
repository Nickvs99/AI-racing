using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Engine)), RequireComponent(typeof(Brakes)), RequireComponent(typeof(SteeringWheel))]
public class CarManager : MonoBehaviour
{
    public Vector2 carInput { get; set; }

    private Rigidbody rb;

    private Engine engine;
    private Brakes brakes;
    private SteeringWheel steeringWheel;
    private Wheel[] wheels;

    // TEMP, will be combined in the carInput 
    public bool isBreaking;

    public float speed;

    private void Awake()
    {
        wheels = GetComponentsInChildren<Wheel>();

        rb = GetComponent<Rigidbody>();

        engine = GetComponent<Engine>();
        brakes = GetComponent<Brakes>();
        steeringWheel = GetComponent<SteeringWheel>();
    }

    private void Update()
    {
        speed = CalcSpeed();
    }

    private void FixedUpdate()
    {
        engine.ApplyForce(carInput.y, wheels);
        brakes.ApplyForce(isBreaking ? 1 : 0, wheels);
        steeringWheel.ApplySteeringAngle(carInput.x, wheels);
    }
    public float CalcSpeed()
    {
        // The speed is the displacement of the rigidbody projected on the forward direction
        float spd = Vector3.Dot(rb.velocity, transform.forward);

        // Convert m/s to km/h
        return spd * 3.6f;
    }
}
