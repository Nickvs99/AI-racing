using PathCreation;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LapTracker), typeof(CarManager))]
public class Agent : PhysicsExtension
{
    [SerializeField] PathCreator pathCreator;
    public VertexPath path;

    private CarManager carManager;
    private LapTracker lapTracker;

    public float crashPenalty = 0.5f;
    private bool hasCrashed = false;
    public bool hasFinished = false;

    [SerializeField] public float maxFuel = 1000f;
    [SerializeField] private float fuelLeft;

    [Header("Vision")]
    [SerializeField] public float fov = 120;
    [SerializeField] public int nrays = 5;

    [Header("Premature Conditions")]
    [SerializeField] private float minAgentHeight = -1f;
    [SerializeField] private float minSpeed = 0.001f;
    [SerializeField] private float minSpeedMaxDuration = 100f;
    [SerializeField] private float fitnessCheckPeriod = 500f;
    private float minSpeedDuration;
    private float prevFitness;

    public NeuralNetwork neuralNetwork;

    private void Awake()
    {
        carManager = GetComponent<CarManager>();
        lapTracker = GetComponent<LapTracker>();
  }

    void Start()
    {
        path = pathCreator.path;
        
        Init();
    }

    public void Init()
    {
        InitTransform();
        lapTracker.Init();
        carManager.Init();

        fuelLeft = maxFuel;
        hasFinished = false;
        hasCrashed = false;

        minSpeedDuration = 0f;
        prevFitness = CalcFitness();
    }

    private void InitTransform()
    {
        // Initialize agent transform
        transform.position = path.GetPoint(0);
        transform.LookAt(path.GetPoint(1));

        // Move agent slightly ahead, oterwise the CalculateClosestPointOnPathData
        // would think it is at the end of the lap
        transform.position += transform.forward * 0.1f;
    }

    public override void PhysicsUpdate()
    {

        // Premature condition checking
        if (transform.position.y < minAgentHeight)
        {
            hasCrashed = true;
        }

        if (Mathf.Abs(carManager.speed) < minSpeed)
        {
            minSpeedDuration += 1f;
        }
        else
        {
            minSpeedDuration = 0f;
        }

        // Check for backwards driving by comparing fitness values against a previous time
        if (fuelLeft % fitnessCheckPeriod == 0)
        {
            float currentFitness = CalcFitness();
            if (currentFitness < prevFitness)
            {
                hasCrashed = true;
            }

            prevFitness = currentFitness;
        }

        fuelLeft -= 1f;

        if (fuelLeft < 0f || hasCrashed || minSpeedDuration >= minSpeedMaxDuration)
        {
            hasFinished = true;
        }

        if (hasFinished)
        {
            return;
        }
        
        float[] neuralNetworkInput = new float[neuralNetwork.GetLayerSize(0)];

        float[] rayDistances = GetVisionDistances();
        for (int i = 0; i < rayDistances.Length; i++)
        {
            neuralNetworkInput[i] = rayDistances[i];
        }

        neuralNetworkInput[neuralNetworkInput.Length - 1] = carManager.speed;

        carManager.carInput = ComputeCarInput(neuralNetworkInput);
        
        lapTracker.CheckLapCompleted();

        if (!Physics.autoSimulation)
        {
            carManager.PhysicsUpdate();
        }
    }

    public void SetNeuralNetwork(NeuralNetwork network)
    {
        neuralNetwork = network;
    }

    private CarInput ComputeCarInput(float [] neuralNetworkInput)
    {
        float[] neuralNetworkOutput = neuralNetwork.ComputeOutputs(neuralNetworkInput);
        return new CarInput(neuralNetworkOutput[0], neuralNetworkOutput[1], neuralNetworkOutput[2]);
    }

    // Should only be computed at the end of a run. Should not be used every frame
    public float CalcFitness()
    {
        // Fitness for completed laps
        float lapFitness = lapTracker.currentLap * (path.NumPoints - 1);

        // Fitness for partial laps
        TimeOnPathData data = path.CalculateClosestPointOnPathData(transform.position);
        float partialFitness = data.previousIndex + data.percentBetweenIndices;

        // Penalty to fitness for crashing
        float multiplier = 1;
        if(hasCrashed)
        {
            multiplier = (lapFitness + partialFitness) > 0f ? crashPenalty : 1f / crashPenalty;
        }

        return (lapFitness + partialFitness) * multiplier;
    }

    private float[] GetVisionDistances()
    {
        float[] visionDistances = new float[nrays];

        float startAngle = -fov / 2;
        float angleIncrement = fov / (nrays - 1);

        Vector3 origin = transform.position + Vector3.up;

        LayerMask mask = LayerMask.GetMask("TrackWall");

        for(int i = 0; i < nrays; i++ )
        {
            float angle = startAngle + angleIncrement * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            if(Physics.Raycast(origin, dir, out hit, Mathf.Infinity, mask))
            {
                DrawVisionRay(origin, dir, hit.distance);
                visionDistances[i] = hit.distance;
            }
        }

        return visionDistances;
    }

    private void DrawVisionRay(Vector3 origin, Vector3 dir, float distance)
    {
        Color color;
        if (distance < 30f)
        {
            color = Color.red;
        }
        else if (distance < 75f)
        {
            color = Color.yellow;
        }
        else
        {
            color = Color.green;
        }

        Debug.DrawRay(origin, dir * distance, color);
    }

    public void Save()
    {
        SaveManager.SaveAgent(new AgentData(this));
    }

    public void Load(AgentData data)
    {
        fov = data.fov;
        nrays = data.nrays;
        neuralNetwork = data.neuralNetwork;
    }
}
