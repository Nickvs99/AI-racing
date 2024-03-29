using System;

[Serializable]
public class NeuralNetwork
{
    public int nLayers;
    public int[] layerSizes;
    public NeuralNetworkLayer[] layers;

    public NeuralNetwork(int[] _layerSizes, string weightInitName = "Default", string biasInitName = "Default", string activationName = "Default")
    {
        layerSizes = _layerSizes;
        nLayers = layerSizes.Length;

        layers = new NeuralNetworkLayer[nLayers];
        for (int i = 0; i < nLayers; i++)
        {
            int nInputs = layerSizes[i];

            int nOutputs;
            if (i == nLayers - 1)
            {
                // Last layer does not have any outputs
                nOutputs = 0;
            }
            else
            {
                nOutputs = layerSizes[i + 1];
            }

            Func<float> weightInitMethod = InitializationTable.weightTable[weightInitName];
            Func<float> biasInitMethod = InitializationTable.biasTable[biasInitName];
            layers[i] = new NeuralNetworkLayer(nInputs, nOutputs, weightInitMethod, biasInitMethod, activationName);
        }
    }

    public int GetLayerSize(int i)
    {
        return layerSizes[i];
    }

    public float[] ComputeOutputs(float[] inputs)
    {
        foreach(NeuralNetworkLayer layer in layers)
        {
            // The output of a layer, becomes the input for the next layer
            inputs = layer.ComputeOutputs(inputs);
        }

        return inputs;
    }

    public NeuralNetwork Clone()
    {
        NeuralNetwork clone = new NeuralNetwork(layerSizes);
        
        for(int i = 0; i < nLayers; i++)
        {
            clone.layers[i] = layers[i].Clone();
        }

        return clone;
    }

    // Testing purposes and possibly usefull for biodiversity
    public Tuple<float, float> ComputeTotalWeightAndBias()
    {
        float totalWeight = 0f;
        float totalBias = 0f;

        foreach(NeuralNetworkLayer layer in layers)
        {
            (float weight, float bias) = layer.ComputeTotalWeightAndBias();
            totalWeight += weight;
            totalBias += bias;
        }

        return new Tuple<float, float>(totalWeight, totalBias);
    }

    public void Mutate(Func<float, float, float> WeightMutateMethod, Func<float, float, float> BiasMutateMethod, float mutateProbability)
    {
        for(int i = 0; i < layers.Length - 1; i++)
        {
            layers[i].Mutate(WeightMutateMethod, BiasMutateMethod, mutateProbability);
        }
    }
}
