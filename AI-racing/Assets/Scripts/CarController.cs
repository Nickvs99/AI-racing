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
        // TEMP, will be set through the car controller
        manager.carInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        manager.isBreaking = Input.GetKey(KeyCode.Space);
    }
}