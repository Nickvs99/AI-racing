using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EvolutionManager : PhysicsExtension
{
    [SerializeField] EvolutionProgressDisplay display;
    [SerializeField] private Agent agent;

    [SerializeField] private int populationSize = 5;
    public float mutateProbability = 0.1f;

    private NeuralNetwork[] neuralNetworks;
    public float[] fitnesses;

    public int generation;
    public int currentAgentIndex;

    [Header("Seeding")]
    [SerializeField] private bool useFixedSeed = false;
    [SerializeField] private int seed = 0;

    [Header("Neural Network")]
    [SerializeField] private int[] hiddenLayerSizes;
    private int[] layerSizes;

    [Header("Data logger")]
    [SerializeField] private bool loggerEnabled = true;
    [SerializeField] private string pathFromAssets = "../../data/data.txt";
    [SerializeField] [TextArea(3, 10)] private string customComment = "";
    private DataLogger dataLogger;

    float overallBest = Mathf.NegativeInfinity;
    float overallWorst = Mathf.Infinity;
    float overallBestAvg = Mathf.NegativeInfinity;



    private void Start()
    {
        InitRNG();

        neuralNetworks = new NeuralNetwork[populationSize];
        fitnesses = new float[populationSize];

        generation = 0;
        currentAgentIndex = 0;

        // Determine layersizes of the neural networks
        List<int> layerSizesList = new List<int> { agent.nrays + 1 }; // Input (vision + car speed)
        layerSizesList.AddRange(hiddenLayerSizes);                    // Hidden layers
        layerSizesList.Add(3);                                        // Output (engine, brake, steering)

        layerSizes = layerSizesList.ToArray();

        // Create initial networks
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

        if(loggerEnabled)
        {
            InitLogger();
        }

        Physics.autoSimulation = false;
    }

    private void Update()
    {
        // Toggle learn rate
        if(Input.GetKeyDown("p"))
            Physics.autoSimulation = !Physics.autoSimulation;

        if(!Physics.autoSimulation)
            Run();
    }

    public void Run()
    {
        for (int i = 0; i < 100; i++)
        {
            PhysicsUpdate();

            // Physics.autoSimulation might have been turned off in the physics update
            if(Physics.autoSimulation)
            {
                return;
            }

            Physics.Simulate(Time.fixedDeltaTime);
        }
    }
    
    private void InitRNG()
    {
        if (!useFixedSeed)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }

        UnityEngine.Random.InitState(seed);
    }

    private void InitLogger()
    {
        (string, Func<string>)[] logMethod = new (string, Func<string>)[]
        {
            ("Generation", () => {return generation.ToString(); }),
            ("Best", () => {return fitnesses.Max().ToString(); }),
            ("Avg", () => {return fitnesses.Average().ToString(); }),
            ("Worst", () => {return fitnesses.Min().ToString(); }),
        };

        string initialText = $@"{customComment}
{new String('-', 40)}
population size: {populationSize}
mutate probability: {mutateProbability}
hidden layer sizes: {string.Join(", ", hiddenLayerSizes)}

Agent details
fov: {agent.fov}
nrays: {agent.nrays}
max fuel: {agent.maxFuel}";

        string path = Path.Combine(Application.dataPath, pathFromAssets);
        dataLogger = new DataLogger(logMethod, path, initialText: initialText);
    }

    public override void PhysicsUpdate()
    {
        if (agent.hasFinished)
        {
            float fitness = agent.CalcFitness();
            fitnesses[currentAgentIndex] = fitness;

            // If last agent of its generation has finished, create new generation
            if (currentAgentIndex == populationSize - 1)
            {
                float currentBest = fitnesses.Max();
                float currentWorst = fitnesses.Min();
                float currentAvg = fitnesses.Average();

                overallBest = Mathf.Max(overallBest, currentBest);
                overallWorst = Mathf.Min(overallWorst, currentWorst);
                overallBestAvg = Mathf.Max(overallBestAvg, currentAvg);

                display.UpdatePreviousGenerationField(currentBest, currentWorst, currentAvg);
                display.UpdateOverallField(overallBest, overallWorst, overallBestAvg);

                if (loggerEnabled)
                {
                    dataLogger.Log();
                }
                neuralNetworks = CreateNextGeneration();
                generation++;

                if(generation == 10)
                {
                    Physics.autoSimulation = true;
                }
            }

            currentAgentIndex = (currentAgentIndex + 1) % populationSize;

            display.UpdateGenerationProgressField(generation, currentAgentIndex);

            agent.Init();
            agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
        }

        if(!Physics.autoSimulation)
        {
            agent.PhysicsUpdate();
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

    private void MutateNetworks(NeuralNetwork[] networks)
    {
        for(int i = 0; i < networks.Length; i++)
        {
            MutateNetwork(networks[i]);
        }
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
            mutation = UnityEngine.Random.Range(-0.5f, 0.5f);
        }
        return weight + mutation;
    }

    private float BiasMutateMethod(float bias)
    {
        float mutation = 0;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < mutateProbability)
        {
            mutation = UnityEngine.Random.Range(-0.2f, 0.2f);
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
