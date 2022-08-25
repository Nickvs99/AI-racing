using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarController : MonoBehaviour
{
    private CarManager manager;

    private void Awake()
    {
        manager = GetComponent<CarManager>();
    }

    private void Update()
    {
        float engineInput;
        float brakeInput;
        if (manager.speed >= 0)
        {
            engineInput = Input.GetAxis("Positive Acceleration");
            brakeInput = Input.GetAxis("Negative Acceleration");
        }
        else
        {
            engineInput = -Input.GetAxis("Negative Acceleration");
            brakeInput = Input.GetAxis("Positive Acceleration");
        }

        manager.carInput = new CarInput(
            engineInput,
            brakeInput,
            Input.GetAxis("Steering")
       );
    }
}