using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CarInput
{
    // Ranges from -1 (reverse) to 1 (acceleration)
    public float accelerationInput;
    
    // Ranges from 0 (no brakes) to 1 (max brakes)
    public float brakeInput;

    // Ranges from -1 (left) to 1 (right)
    public float steerInput;

    public CarInput(float _accelerationInput, float _brakeInput, float _steerInput)
    {
        accelerationInput = _accelerationInput;
        brakeInput = _brakeInput;
        steerInput = _steerInput;
    }
}
