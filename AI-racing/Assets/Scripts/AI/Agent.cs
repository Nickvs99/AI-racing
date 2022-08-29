using PathCreation;
using UnityEngine;

[RequireComponent(typeof(LapTracker))]
public class Agent : MonoBehaviour
{
    [SerializeField] PathCreator pathCreator;
    public VertexPath path;

    private LapTracker lapTracker;

    public float crashPenalty = 1000;
    private bool hasCrashed = false;

    private void Awake()
    {
        lapTracker = GetComponent<LapTracker>();
    }

    void Start()
    {
        path = pathCreator.path;
        
        // Initialize agent transform
        transform.position = path.GetPoint(0);
        transform.LookAt(path.GetPoint(1));

        // Move agent slightly ahead, oterwise the CalculateClosestPointOnPathData
        // would think it is at the end of the lap
        transform.position += transform.forward * 0.1f;
    }

    private void FixedUpdate()
    {
        if(!hasCrashed && transform.position.y < -1)
        {
            hasCrashed = true;
        }
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
}
