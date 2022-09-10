using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Engine)), RequireComponent(typeof(Brakes)), RequireComponent(typeof(SteeringWheel))]
public class CarManager : MonoBehaviour, IPhysicsObject
{
    public CarInput carInput { get; set; }

    private Rigidbody rb;

    private Engine engine;
    private Brakes brakes;
    private SteeringWheel steeringWheel;
    private Wheel[] wheels;

    public float speed;

    private void Awake()
    {
        wheels = GetComponentsInChildren<Wheel>();

        rb = GetComponent<Rigidbody>();

        engine = GetComponent<Engine>();
        brakes = GetComponent<Brakes>();
        steeringWheel = GetComponent<SteeringWheel>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach(Wheel wheel in wheels)
        {
            wheel.Init();
        }

        carInput = new CarInput(0f, 0f, 0f);
        speed = 0f;
    }

    private void Update()
    {
        speed = CalcSpeed();
    }

    private void FixedUpdate()
    {
        if (Physics.autoSimulation)
        {
            PhysicsStep();
        }
    }

    public void PhysicsStep()
    {
        speed = CalcSpeed();
        
        engine.ApplyForce(carInput.engineInput, wheels);
        brakes.ApplyForce(carInput.brakeInput, wheels);
        steeringWheel.ApplySteeringAngle(carInput.steerInput, wheels);
    }

    public float CalcSpeed()
    {
        // The speed is the displacement of the rigidbody projected on the forward direction
        float spd = Vector3.Dot(rb.velocity, transform.forward);

        // Convert m/s to km/h
        return spd * 3.6f;
    }
}
