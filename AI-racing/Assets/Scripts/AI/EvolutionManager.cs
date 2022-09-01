using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] EvolutionProgressDisplay display;

    [SerializeField] private int populationSize = 5;
    [SerializeField] private Agent agent;

    private NeuralNetwork[] neuralNetworks;
    public float[] fitnesses;

    public int generation;
    public int currentAgentIndex;

    [Header("Neural Network")]
    [SerializeField] private int[] hiddenLayerSizes;
    private int[] layerSizes;

    public float mutateProbability = 0.1f;

    float overallBest = Mathf.NegativeInfinity;
    float overallWorst = Mathf.Infinity;
    float overallBestAvg = Mathf.NegativeInfinity;

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

        display.UpdateGenerationProgressField(generation, currentAgentIndex);
        display.UpdatePreviousGenerationField(0f, 0f, 0f);
        display.UpdateOverallField(0f, 0f, 0f);
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
                float currentBest = fitnesses.Max();
                float currentWorst = fitnesses.Min();
                float currentAvg = fitnesses.Average();

                overallBest = Mathf.Max(overallBest, currentBest);
                overallWorst = Mathf.Min(overallWorst, currentWorst);
                overallBestAvg = Mathf.Max(overallBestAvg, currentAvg);

                display.UpdatePreviousGenerationField(currentBest, currentWorst, currentAvg);
                display.UpdateOverallField(overallBest, overallWorst, overallBestAvg);

                neuralNetworks = CreateNextGeneration();
                generation++;
            }

            currentAgentIndex = (currentAgentIndex + 1) % populationSize;

            display.UpdateGenerationProgressField(generation, currentAgentIndex);

            agent.Init();
            agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
        }
    }

    private NeuralNetwork[] CreateNextGeneration()
    {

        NeuralNetwork[] networks = NeuralNetworkSelection();
        MutateNetworks(networks);

        return networks;
    }

    /// <summary>
    /// Select the best performing neural networks. Their pick probability is proportional to their fitness difference
    /// to the lowest fitness.
    /// </summary>
    /// <returns></returns>
    private NeuralNetwork[] NeuralNetworkSelection()
    {
        NeuralNetwork[] networks = new NeuralNetwork[populationSize];

        // Cumulate fitness values
        float[] cumFitnesses = new float[populationSize];
        float cumFitness = 0;

        float worstFitness = fitnesses.Min();
        for(int i = 0; i < populationSize; i++)
        {
            // Worst fitness is substracted to allow negative fitness values. Also increases probability of higher
            // performing neural networks once all fitness values are high.
            cumFitness += fitnesses[i] - worstFitness;
            cumFitnesses[i] = cumFitness;
        }

        // Normalise fitness
        for(int i=0; i < populationSize; i++)
        {
            cumFitnesses[i] /= cumFitness;
        }

        for (int i = 0; i < populationSize; i++)
        {
            float r = UnityEngine.Random.Range(0f, 1f);

            // Pick a neural network based on the normalised fitness
            int index;
            for(index = 0; index < populationSize - 1; index++)
            {
                if(r < cumFitnesses[index])
                {
                    break;
                }
            }
            networks[i] = neuralNetworks[index].Clone();
        }

        return networks;
    }

    private NeuralNetwork[] MutateNetworks(NeuralNetwork[] neurals)
    {
        NeuralNetwork[] networks = new NeuralNetwork[neurals.Length];
        
        for(int i = 0; i < neurals.Length; i++)
        {
            MutateNetwork(neurals[i]);
        }

        return networks;
    }

    private void MutateNetwork(NeuralNetwork neuralNetwork)
    {
        neuralNetwork.Mutate(WeightMutateMethod, BiasMutateMethod);
    }

    private float WeightMutateMethod(float weight)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if(r < mutateProbability)
        {
            mutation = UnityEngine.Random.Range(-0.1f, 0.1f);
        }
        return weight + mutation;
    }

    private float BiasMutateMethod(float bias)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < mutateProbability)
        {
            mutation = UnityEngine.Random.Range(-0.1f, 0.1f);
        }
        return bias + mutation;
    }

    private float WeightInitMethod()
    {
        return UnityEngine.Random.Range(-1f, 1f);
    }

    private float ActivationMethod(float x)
    {
        return x;
    }
}
