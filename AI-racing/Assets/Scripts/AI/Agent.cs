using PathCreation;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LapTracker), typeof(CarManager))]
public class Agent : MonoBehaviour
{
    [SerializeField] PathCreator pathCreator;
    public VertexPath path;

    private CarManager carManager;
    private LapTracker lapTracker;

    public float crashPenalty = 1000;
    private bool hasCrashed = false;
    public bool hasFinished = false;

    [SerializeField] public float maxFuel = 1000f;
    [SerializeField] private float fuelLeft;

    [Header("Vision")]
    [SerializeField] private float fov = 120;
    [SerializeField] public int nrays = 5;
    
    private NeuralNetwork neuralNetwork;

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

    private void FixedUpdate()
    {
        float[] neuralNetworkInput = new float[neuralNetwork.GetLayerSize(0)];

        float[] rayDistances = GetVisionDistances();
        for(int i = 0; i < rayDistances.Length; i++)
        {
            neuralNetworkInput[i] = rayDistances[i];
        }

        neuralNetworkInput[neuralNetworkInput.Length - 1] = carManager.speed;

        carManager.carInput = ComputeCarInput(neuralNetworkInput);

        if(!hasCrashed && transform.position.y < -1)
        {
            hasCrashed = true;
        }

        fuelLeft -= 1f;

        if (fuelLeft < 0f || hasCrashed)
        {
            hasFinished = true;
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

        float penalty = hasCrashed ? crashPenalty : 0f;

        return lapFitness + partialFitness - penalty;
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
            else
            {
                Debug.LogWarning("Ray fails to see anyting", gameObject);
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
}
