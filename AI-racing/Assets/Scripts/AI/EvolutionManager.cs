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
    public string selectionName = "Default";
    
    [Header("Mutation parameters")]
    public float mutateProbability = 0.1f;
    public string weightMutateName = "Default";
    public string biasMutateName = "Default";

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

    private float overallBest = Mathf.NegativeInfinity;
    private float overallWorst = Mathf.Infinity;
    private float overallBestAvg = Mathf.NegativeInfinity;

    private NeuralNetwork overallBestNeuralNetwork;

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
                weightInitName: "Default",
                biasInitName: "Default",
                activationName: "Default"
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

        // Save last completed agent
        if(Input.GetKeyDown("s"))
        {
            AgentData data = new AgentData(agent, overallBestNeuralNetwork);
            SaveManager.SaveAgent(data);
        }

        // Load
        if(Input.GetKeyDown("l"))
        {
            AgentData data = SaveManager.LoadAgentData();

            agent.Load(data);
            agent.Init();

            Physics.autoSimulation = true;
        }

        if (!Physics.autoSimulation)
            Run();
    }

    public void Run()
    {
        int i = 0;
        while(!agent.hasFinished && i < 1000)
        {
            PhysicsUpdate();

            // Physics.autoSimulation might have been turned off in the physics update
            if(Physics.autoSimulation)
            {
                return;
            }

            Physics.Simulate(Time.fixedDeltaTime);
        }

        if(agent.hasFinished)
        {
            OnAgentFinish();
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
            OnAgentFinish();
        }

        if(!Physics.autoSimulation)
        {
            agent.PhysicsUpdate();
        }
    }

    private void OnAgentFinish()
    {
        float fitness = agent.CalcFitness();
        fitnesses[currentAgentIndex] = fitness;

        if(fitness > overallBest)
        {
            overallBest = fitness;
            overallBestNeuralNetwork = agent.neuralNetwork.Clone();
        }

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
        }

        currentAgentIndex = (currentAgentIndex + 1) % populationSize;

        display.UpdateGenerationProgressField(generation, currentAgentIndex);

        agent.Init();
        agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
    }

    private NeuralNetwork[] CreateNextGeneration()
    {
        NeuralNetwork[] networks = NeuralNetworkSelection();
        
        MutateNetworks(networks);

        return networks;
    }


    private NeuralNetwork[] NeuralNetworkSelection()
    {
        NeuralNetwork[] networks = new NeuralNetwork[populationSize];

        Func<float[], int[]> SelectionMethod = SelectionTable.table[selectionName];
        int[] indices = SelectionMethod(fitnesses);

        for(int i = 0; i < populationSize; i++)
        {
            networks[i] = neuralNetworks[indices[i]].Clone();
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
        Func<float, float, float> WeightMutateMethod = MutationTable.weightTable[weightMutateName];
        Func<float, float, float> BiasMutateMethod = MutationTable.biasTable[biasMutateName];

        neuralNetwork.Mutate(WeightMutateMethod, BiasMutateMethod, mutateProbability);
    }
}
