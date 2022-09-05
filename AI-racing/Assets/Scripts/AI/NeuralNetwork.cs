using System;

public class NeuralNetwork
{
    private int nLayers;
    private int[] layerSizes;
    private NeuralNetworkLayer[] layers;

    public NeuralNetwork(int[] _layerSizes, Func<float> weightInitMethod = null, Func<float> biasInitMethod = null, Func<float, float> activationMethod = null)
    {
        if(weightInitMethod == null)
        {
            weightInitMethod = DefaultWeightInitMethod;
        }

        if(biasInitMethod == null)
        {
            biasInitMethod = DefaultBiasInitMethod;
        }

        if(activationMethod == null)
        {
            activationMethod = DefaultActivationMethod;
        }

        layerSizes = _layerSizes;
        nLayers = layerSizes.Length;

        // Last layer does not need to be stored, since it is only a output layer
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

            layers[i] = new NeuralNetworkLayer(nInputs, nOutputs, weightInitMethod, biasInitMethod, activationMethod);
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
        
        for(int i = 0; i < nLayers - 1; i++)
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

    public void Mutate(Func<float, float> WeightMutateMethod, Func<float, float> BiasMutateMethod)
    {
        for(int i = 0; i < layers.Length - 1; i++)
        {
            layers[i].Mutate(WeightMutateMethod, BiasMutateMethod);
        }
    }

    public float DefaultWeightInitMethod()
    {
        return 1f;
    }

    public float DefaultBiasInitMethod()
    {
        return 0f;
    }

    public float DefaultActivationMethod(float x)
    {
        return 1f / (1f + (float) Math.Exp(-x));
    }
}
