using UnityEngine;

/// <summary>
/// Every class which implements FixedUpdate should inherit this class. It makes sure
/// that the physics calculations work for autoSimulation turned on and off.
/// </summary>
public abstract class PhysicsExtension : MonoBehaviour
{
    public void FixedUpdate()
    {
        if(Physics.autoSimulation)
        {
            PhysicsUpdate();
        }
    }

    public abstract void PhysicsUpdate();
}
