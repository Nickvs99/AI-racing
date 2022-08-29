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
    }

    public float CalcFitness()
    {   
        // Should only be computed at the end of a run. Should not be used every frame
        TimeOnPathData data = path.CalculateClosestPointOnPathData(transform.position);
        return data.previousIndex + data.percentBetweenIndices;
    }
}
