
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private float maxSteeringAngle;

    public void ApplySteeringAngle(float input, Wheel[] wheels)
    {
        foreach(Wheel wheel in wheels)
        {
            wheel.ApplySteering(input * maxSteeringAngle);
        }
    }
}
