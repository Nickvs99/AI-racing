using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EvolutionParameters
{
    public int populationSize;
    
    public string selectionName;
    public string weightInitName;
    public string biasInitName;
    public string activationName;

    public float mutationRate;
    public string weightMutateName;
    public string biasMutateName;

    public int[] hiddenLayers;

    public EvolutionParameters(int _populationSize, string _selectionName, string _weightInitName, string _biasInitName, string _activationName, 
                    float _mutationRate, string _weightMutateName, string _biasMutateName, int[] _hiddenLayers)
    {
        populationSize = _populationSize;
        selectionName = _selectionName;
        weightInitName = _weightInitName;
        biasInitName = _biasInitName;
        activationName = _activationName;
        mutationRate = _mutationRate;
        weightMutateName = _weightMutateName;
        biasMutateName = _biasMutateName;
        hiddenLayers = _hiddenLayers;
    }

}
