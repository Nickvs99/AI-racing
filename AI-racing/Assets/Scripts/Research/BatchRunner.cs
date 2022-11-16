using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Runs a set of parameter runs. The parameter space is explored through grid search, where all possible 
/// permutations are explored.
/// </summary>
public class BatchRunner : MonoBehaviour
{
    [SerializeField] private string dataLogTablePath = "";
    private DataLogTable dataLogTable;

    [SerializeField] private EvolutionManager manager;

    public int[] populationSizes = new int[] { 5 };

    public string[] selectionNames = new string[] { "Default" };
    public string[] weightInitNames = new string[] { "Default" };
    public string[] biasInitNames = new string[] { "Default" };
    public string[] activationNames = new string[] { "Default" };

    public float[] mutationRates = new float[] { 0.1f };
    public string[] weightMutateNames = new string[] { "Default" };
    public string[] biasMutateNames = new string[] { "Default" };

    public WrapperArray<WrapperArray<int>> hiddenLayersValues;

    public float[] fovs = new float[] { 60f };
    public int[] nrayss = new int[] { 5 };

    private IEnumerable<EvolutionParameters> permutations;
    private IEnumerator enumerator;
    private EvolutionParameters parameters;
    private int currentID;

    private void Awake()
    {
        Physics.autoSimulation = false;
    }

    private void Start()
    {
        permutations = GetAllPermutations();
        enumerator = permutations.GetEnumerator();
        
        string path = Path.Combine(Application.dataPath, dataLogTablePath);
        dataLogTable = new DataLogTable(path);

        manager.loggerEnabled = true;

        InitNextExperiment();
    }

    private void Update()
    {
        if (manager.hasCompleted)
        {
            // Add parameter combination to parameter map file once finished
            dataLogTable.AddNewID(parameters);

            InitNextExperiment();
        }
        else
        {
            manager.Run();
        }
    }

    private void InitNextExperiment()
    {
        // Move to next parameters and check if the end is reached
        if(!enumerator.MoveNext())
        {
            Debug.Log("End of experiments reached!");
            Debug.Break();
            return;
        }

        parameters  = (EvolutionParameters) enumerator.Current;
        Debug.Log($"Current: {parameters}");

        // check if parameter combination exists
        if(dataLogTable.CheckFileExists(parameters))
        {
            manager.hasCompleted = true;
            return;
        }

        currentID = dataLogTable.GetNewID();

        // Set new data log path
        manager.pathFromAssets = $"../../data/data - {currentID}.txt"; // TODO should be set through inspector

        // Initialize manager
        manager.SetParameters(parameters);
        manager.InitManager();
    }

    // TODO refactor, gosh this is ugly and unmaintainable
    private IEnumerable<EvolutionParameters> GetAllPermutations()
    {
        foreach(int populationSize in populationSizes)
        {
            foreach(string selectionName in selectionNames)
            {
                foreach(string weightInitName in weightInitNames)
                {
                    foreach(string biasInitName in biasInitNames)
                    {
                        foreach(string activationName in activationNames)
                        {
                            foreach(float mutationValue in mutationRates)
                            {
                                foreach(string weightMutateName in weightMutateNames)
                                {
                                    foreach(string biasMutateName in biasMutateNames)
                                    {
                                        foreach(WrapperArray<int>  hiddenLayers in hiddenLayersValues)
                                        {
                                            foreach(float fov in fovs)
                                            {
                                                foreach(int nrays in nrayss)
                                                {
                                                    yield return new EvolutionParameters(populationSize, selectionName, weightInitName, biasInitName,
                                                            activationName, mutationValue, weightMutateName, biasMutateName, hiddenLayers.array, fov, nrays);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        yield break;
    }
}

