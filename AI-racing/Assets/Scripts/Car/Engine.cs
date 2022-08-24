using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] private float maxForce;

    public void ApplyForce(float input, Wheel[] wheels)
    {
        foreach(Wheel wheel in wheels)
        {
            wheel.ApplyMotorForce(input * maxForce);
        }
    }
}
