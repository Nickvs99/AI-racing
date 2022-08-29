using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class LapTracker : MonoBehaviour
{

    [SerializeField] PathCreator pathCreator;

    public VertexPath path;

    public int currentLap;

    private Vector3 prevPosition;

    private Vector3 finishCenter;

    // Two vectors used to determine the crossing of the finish line
    private Vector3 finishA;
    private Vector3 finishB;
    private float deltaX;
    private float deltaZ;

    void Start()
    {
        path = pathCreator.path;
        currentLap = 0;
        prevPosition = transform.position;

        Vector3 normal = path.GetNormal(0);
        finishCenter = path.GetPoint(0);
        finishA = finishCenter + normal;
        finishB = finishCenter - normal;

        // Compute non changing values
        deltaX = finishB.x - finishA.x;
        deltaZ = finishB.z - finishA.z;
    }

    private void FixedUpdate()
    {
        // Only check if the distance is smaller than  twice the roadwidth of the road mesh
        // The roadwidth on the script is actually the road radius, therefore twice.
        // TODO set treshold based on roadwidth
        if (Vector3.Distance(transform.position, finishCenter) < 20)
        {
            bool isRightPrev = isPositionRightOfFinish(prevPosition);
            bool isRightCurrent = isPositionRightOfFinish(transform.position);

            // If not equal than a lap is driven
            if (isRightPrev != isRightCurrent)
            {
                currentLap += isRightCurrent ? 1 : -1;
            }
        }

        prevPosition = transform.position;
    }

    /// <summary>
    /// Determines if a given position is left of the finish line. 
    /// </summary>
    /// <see href="https://math.stackexchange.com/a/274728"/>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool isPositionRightOfFinish(Vector3 position)
    {
        float signedDist = (position.x - finishA.x) * deltaZ - (position.z - finishB.z) * deltaX;
        return signedDist > 0 ? true : false;
    }
}
