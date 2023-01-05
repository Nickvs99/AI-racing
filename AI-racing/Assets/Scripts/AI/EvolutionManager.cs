using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EvolutionManager : PhysicsExtension
{
    [SerializeField] private int nRuns = 1;
    
    private int currentRun;

    [SerializeField] EvolutionProgressDisplay display;
    [SerializeField] private Agent agent;

    [SerializeField] private int populationSize = 5;

    [Header("Methods")]
    public string selectionName = "Default";
    public string weightInitName = "Default";
    public string biasInitname = "Default";
    public string activationName = "Default";
    public string terminationName = "Default";

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
    public bool loggerEnabled = true;
    public string pathFromAssets = "../../data/data.txt";
    [SerializeField] [TextArea(3, 10)] private string customComment = "";
    private DataLogger dataLogger;

    private float runBest;
    private float runWorst;
    private float runBestAvg;

    private List<float> avgFitnesses;
    private List<float> maxFitnesses;

    private NeuralNetwork overallBestNeuralNetwork;

    public bool hasCompleted = false;

    private void Start()
    {
        InitRNG();

        // TODO: Needs to be a variable
        //Needed when the evolution manager is not ran through the batchrunner
        //InitManager();
    }

    public void InitManager()
    {
        hasCompleted = false;

        neuralNetworks = new NeuralNetwork[populationSize];
        fitnesses = new float[populationSize];

        currentRun = 0;

        // Determine layersizes of the neural networks
        List<int> layerSizesList = new List<int> { agent.nrays + 1 }; // Input (vision + car speed)
        layerSizesList.AddRange(hiddenLayerSizes);                    // Hidden layers
        layerSizesList.Add(3);                                        // Output (engine, brake, steering)

        layerSizes = layerSizesList.ToArray();

        InitRun();

        if (loggerEnabled)
        {
            InitLogger();
        }
    }

    private void InitRun()
    {
        generation = 0;
        currentAgentIndex = 0;

        avgFitnesses = new List<float>();
        maxFitnesses = new List<float>();

        InitRunScores();

        // Create initial networks
        for (int i = 0; i < populationSize; i++)
        {
            neuralNetworks[i] = new NeuralNetwork(layerSizes,
                weightInitName: weightInitName,
                biasInitName: biasInitname,
                activationName: activationName
            );
        }

        agent.Init();
        agent.SetNeuralNetwork(neuralNetworks[0]);

        display.UpdateRunField(currentRun);
        display.UpdateGenerationProgressField(generation, currentAgentIndex);
        display.UpdatePreviousGenerationField(0f, 0f, 0f);
        display.UpdateOverallField(0f, 0f, 0f);
    }

    public void SetParameters(EvolutionParameters parameters)
    {
        populationSize = parameters.populationSize;
        selectionName = parameters.selectionName;
        weightInitName = parameters.weightInitName;
        biasInitname = parameters.biasInitName;
        activationName = parameters.activationName;
        mutateProbability = parameters.mutationRate;
        weightMutateName = parameters.weightInitName;
        biasMutateName = parameters.biasMutateName;
        hiddenLayerSizes = parameters.hiddenLayers;

        agent.fov = parameters.fov;
        agent.nrays = parameters.nrays;
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

        // Needed when the evolution manager is not ran through the batchrunner
        //if (!Physics.autoSimulation)
        //    Run();
    }

    public void Run()
    {
        while(!agent.hasFinished)
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
{DataLogger.GetSeparatorRow()}
seed: {seed}

nruns: {nRuns}

population size: {populationSize}
selection method: {selectionName}

mutate bias method: {biasMutateName}
mutate weight method: {weightMutateName}
mutate probability: {mutateProbability}

init bias method: {biasInitname}
init weight method: {weightInitName}
activation method: {activationName}
hidden layer sizes: [{string.Join(", ", hiddenLayerSizes)}]

Agent details
fov: {agent.fov}
nrays: {agent.nrays}
max fuel: {agent.maxFuel}";

        string path = Path.Combine(Application.dataPath, pathFromAssets);
        dataLogger = new DataLogger(logMethod, path, initialText: initialText);
    }
    
    private void InitRunScores()
    {
        runBest = Mathf.NegativeInfinity;
        runWorst = Mathf.Infinity;
        runBestAvg = Mathf.NegativeInfinity;
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

        if(fitness > runBest)
        {
            runBest = fitness;
            overallBestNeuralNetwork = agent.neuralNetwork.Clone();
        }

        // If last agent of its generation has finished, create new generation
        if (currentAgentIndex == populationSize - 1)
        {
            OnGenerationFinish();
            return;
        }

        currentAgentIndex++;

        display.UpdateGenerationProgressField(generation, currentAgentIndex);

        agent.Init();
        agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
    }

    private void OnGenerationFinish()
    {
        float currentBest = fitnesses.Max();
        float currentWorst = fitnesses.Min();
        float currentAvg = fitnesses.Average();

        avgFitnesses.Add(currentAvg);
        maxFitnesses.Add(currentBest);

        runBest = Mathf.Max(runBest, currentBest);
        runWorst = Mathf.Min(runWorst, currentWorst);
        runBestAvg = Mathf.Max(runBestAvg, currentAvg);

        display.UpdatePreviousGenerationField(currentBest, currentWorst, currentAvg);
        display.UpdateOverallField(runBest, runWorst, runBestAvg);

        if (loggerEnabled)
        {
            dataLogger.Log();
        }

        if(TerminationTable.table[terminationName](avgFitnesses, maxFitnesses))
        {
            OnRunFinish();
            return;
        }

        neuralNetworks = CreateNextGeneration();

        currentAgentIndex = 0;
        generation++;

        agent.Init();
        agent.SetNeuralNetwork(neuralNetworks[currentAgentIndex]);
        display.UpdateGenerationProgressField(generation, currentAgentIndex);
    }

    private void OnRunFinish()
    {
        if (currentRun == nRuns - 1)
        {
            hasCompleted = true;
            return;
        }

        currentRun += 1;

        InitRun();

        dataLogger.WriteRowSeparator();
        dataLogger.WriteHeader();
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
