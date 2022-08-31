using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] private int populationSize = 5;
    [SerializeField] private Agent agent;

    private NeuralNetwork[] neuralNetworks;
    private float[] fitnesses;

    public int generation;
    public int currentAgentIndex;

    [Header("Neural Network")]
    [SerializeField] private int[] hiddenLayerSizes;
    private int[] layerSizes;


    private void Start()
    {
        neuralNetworks = new NeuralNetwork[populationSize];
        fitnesses = new float[populationSize];

        generation = 0;
        currentAgentIndex = 0;

        List<int> layerSizesList = new List<int> { agent.nrays + 1 }; // Input (vision + car speed)
        layerSizesList.AddRange(hiddenLayerSizes);                    // Hidden layers
        layerSizesList.Add(3);                                        // Output (engine, brake, steering)

        layerSizes = layerSizesList.ToArray();

        for (int i = 0; i < populationSize; i++)
        {
            neuralNetworks[i] = new NeuralNetwork(layerSizes,
                weightInitMethod: WeightInitMethod,
                activationMethod: ActivationMethod
            );
        }

        agent.SetNeuralNetwork(neuralNetworks[0]);
    }

    private void FixedUpdate()
    {
        if (agent.hasFinished)
        {
            float fitness = agent.CalcFitness();
            fitnesses[currentAgentIndex] = fitness;

            // If last agent of its generation has finished, create new generation
            if(currentAgentIndex == populationSize - 1)
            {
                Debug.Log(string.Join(", ", fitnesses));
                neuralNetworks = CreateNextGeneration();
                generation++;
            }

            currentAgentIndex = (currentAgentIndex + 1) % populationSize;

            agent.Init();
            agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
        }
    }

    private NeuralNetwork[] CreateNextGeneration()
    {
        NeuralNetwork[] networks = new NeuralNetwork[populationSize];
        
        for (int i = 0; i < populationSize; i++)
        {
            networks[i] = new NeuralNetwork(layerSizes,
                weightInitMethod: WeightInitMethod,
                activationMethod: ActivationMethod
            );
        }

        return networks;
    }

    private float WeightInitMethod()
    {
        return Random.Range(-1f, 1f);
    }

    private float ActivationMethod(float x)
    {
        return x;
    }
}
