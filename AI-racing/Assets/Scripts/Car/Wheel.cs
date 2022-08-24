using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    [SerializeField] private bool applyBrakes;
    [SerializeField] private bool applyMotorForce;
    [SerializeField] private bool applySteering;

    private WheelCollider wheelCollider;
    [SerializeField] private Transform wheelVisualTransform;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        UpdateTransform();
    }

    public void ApplyMotorForce(float force)
    {
        if (!applyMotorForce) return;

        wheelCollider.motorTorque = force;
    }

    public void ApplyBrakeForce(float force)
    {
        if (!applyBrakes) return;

        wheelCollider.brakeTorque = force;
    }

    public void ApplySteering(float angle)
    {
        if (!applySteering) return;

        wheelCollider.steerAngle = angle;
    }

    private void UpdateTransform()
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelVisualTransform.rotation = rot;
        wheelVisualTransform.position = pos;
    }
}
