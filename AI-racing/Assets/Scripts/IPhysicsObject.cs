using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement this interface for objects which are handled both through the FixedUpdate and the
/// manual simulation of Physics.Simulate.
/// </summary>
public interface IPhysicsObject
{

    public void PhysicsStep();

}
