using PathCreation;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] PathCreator pathCreator;
    public VertexPath path;

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

    public float CalcFitness()
    {   
        // Should only be computed at the end of a run. Should not be used every frame
        TimeOnPathData data = path.CalculateClosestPointOnPathData(transform.position);
        return data.previousIndex + data.percentBetweenIndices;
    }
}
