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
        float accelerationInput;
        float brakeInput;
        if (manager.speed >= 0)
        {
            accelerationInput = Input.GetAxis("Positive Acceleration");
            brakeInput = Input.GetAxis("Negative Acceleration");
        }
        else
        {
            accelerationInput = -Input.GetAxis("Negative Acceleration");
            brakeInput = Input.GetAxis("Positive Acceleration");
        }

        manager.carInput = new CarInput(
            accelerationInput,
            brakeInput,
            Input.GetAxis("Steering")
       );
    }
}