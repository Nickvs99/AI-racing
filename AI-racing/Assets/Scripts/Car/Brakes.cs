using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brakes : MonoBehaviour
{
    [SerializeField] private float maxForce;

    public void ApplyForce(float input, Wheel[] wheels)
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.ApplyBrakeForce(input * maxForce);
        }
    }
}
