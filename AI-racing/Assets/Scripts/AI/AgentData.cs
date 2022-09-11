using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AgentData
{
    public float fov;
    public int nrays;

    public NeuralNetwork neuralNetwork;

    public AgentData(Agent agent)
    {
        fov = agent.fov;
        nrays = agent.nrays;
        neuralNetwork = agent.neuralNetwork;
    }

    public AgentData(Agent agent, NeuralNetwork network)
    {
        fov = agent.fov;
        nrays = agent.nrays;
        neuralNetwork = network;
    }
}
