using System;

[Serializable]
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
        layers = new NeuralNetworkLayer[nLayers - 1];
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new NeuralNetworkLayer(layerSizes[i], layerSizes[i + 1], weightInitMethod, biasInitMethod, activationMethod);
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
        return (NeuralNetwork) this.MemberwiseClone();
    }

    public void Mutate(Func<float, float> WeightMutateMethod, Func<float, float> BiasMutateMethod)
    {
        for(int i = 0; i<layers.Length - 1; i++)
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
