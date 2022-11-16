using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs a set of parameter runs. The parameter space is explored through grid search, where all possible 
/// permutations are explored.
/// </summary>
public class BatchRunner : MonoBehaviour
{
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

    private IEnumerable<EvolutionParameters> permutations;
    private IEnumerator enumerator;

    private void Awake()
    {
        Physics.autoSimulation = false;
    }

    private void Start()
    {
        permutations = GetAllPermutations();
        enumerator = permutations.GetEnumerator();

        InitNextExperiment();
    }

    private void Update()
    {
        if (manager.hasCompleted)
        {
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

        EvolutionParameters parameters  = (EvolutionParameters) enumerator.Current;
        Debug.Log($"{parameters.populationSize}, {parameters.selectionName}, {parameters.mutationRate}");

        // TODO check if parameter combination exists

        // TODO add parameter combination to parameter map file


        // Set parameters
        manager.SetParameters(parameters);

        // Init run
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
                                            yield return new EvolutionParameters(populationSize, selectionName, weightInitName, biasInitName,
                                                activationName, mutationValue, weightMutateName, biasMutateName, hiddenLayers.array);
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

