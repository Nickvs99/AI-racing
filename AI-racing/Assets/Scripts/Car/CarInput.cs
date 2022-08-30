using UnityEngine;

public struct CarInput
{
    // Ranges from -1 (reverse) to 1 (acceleration)
    public float engineInput;
    
    // Ranges from 0 (no brakes) to 1 (max brakes)
    public float brakeInput;

    // Ranges from -1 (left) to 1 (right)
    public float steerInput;

    public CarInput(float _engineInput, float _brakeInput, float _steerInput)
    {
        engineInput = Mathf.Clamp(_engineInput, -1, 1);
        brakeInput = Mathf.Clamp(_brakeInput, 0, 1);
        steerInput = Mathf.Clamp(_steerInput, -1, 1);
    }
}
