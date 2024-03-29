using System;

[Serializable]
public class NeuralNetworkLayer
{
    public int nInputs;
    public int nOutputs;

    
    public WrapperArray<float>[] weights; // WrapperArray[] can be serialized as opposed to T[,]
    public float[] biases;

    public string activationName;

    // Use this constructor if the weights and biases are manually set
    public NeuralNetworkLayer(int _nInputs, int _nOutputs)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = new WrapperArray<float>[nInputs];
        biases = new float[nOutputs];
    }
    
    public NeuralNetworkLayer(int _nInputs, int _nOutputs, Func<float> weightInitMethod, Func<float> biasInitMethod, string _activationName)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = InitWeights(weightInitMethod);
        biases = InitBiases(biasInitMethod);

        activationName = _activationName;
    }

    public NeuralNetworkLayer Clone()
    {
        // Copy weights from this instance
        WrapperArray<float>[] _weights = new WrapperArray<float>[nInputs];
        for (int i = 0; i < nInputs; i++)
        {
            WrapperArray<float> wrapper = new WrapperArray<float>(new float[nOutputs]);

            for (int j = 0; j < nOutputs; j++)
            {
                wrapper[j] = weights[i][j];
            }

            _weights[i] = wrapper;
        }

        NeuralNetworkLayer clone = new NeuralNetworkLayer(nInputs, nOutputs);

        clone.weights = _weights;

        Array.Copy(biases, clone.biases, biases.Length);

        clone.activationName = activationName;

        return clone;
    }

    private WrapperArray<float>[] InitWeights(Func<float> InitMethod)
    {
        WrapperArray<float>[] _weights = new WrapperArray<float>[nInputs];

        for (int i = 0; i < nInputs; i++)
        {
            WrapperArray<float> wrapper = new WrapperArray<float>(new float[nOutputs]);

            for (int j = 0; j < nOutputs; j++)
            {
                wrapper[j] = InitMethod();
            }

            _weights[i] = wrapper;
        }

        return _weights;
    }

    private float[] InitBiases(Func<float> InitMethod)
    {
        float[] _biases = new float[nOutputs];

        for(int i = 0; i < _biases.Length; i++)
        {
            _biases[i] = InitMethod();
        }

        return _biases;
    }

    public void Mutate(Func<float, float, float> WeightMutateMethod, Func<float, float, float> BiasMutateMethod, float mutateProbability)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = WeightMutateMethod(weights[i][j], mutateProbability);
            }
        }

        for(int i = 0; i < biases.Length; i++)
        {
            biases[i] = BiasMutateMethod(biases[i], mutateProbability);
        }
    }
    
    public float[] ComputeOutputs(float[] inputs)
    {
        if(nOutputs == 0)
        {
            return inputs;
        }

        float[] outputs = new float[nOutputs];

        for(int i = 0; i < nOutputs; i++)
        {
            float value = 0;
            for(int j = 0; j < nInputs; j++)
            {
                value += weights[j][i] * inputs[j];
            }

            outputs[i] = ActivationTable.table[activationName](value + biases[i]);
        }

        return outputs;
    }

    public (float, float) ComputeTotalWeightAndBias()
    {
        float weightTotal = 0;
        float biasTotal = 0;

        foreach (WrapperArray<float> wrapper in weights)
        {
            foreach(float weight in wrapper)
            {
                weightTotal += weight;
            }
        }

        foreach (float bias in biases)
        {
            biasTotal += bias;
        }

        return (weightTotal, biasTotal);
    }
}
