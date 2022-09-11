using System;
using UnityEngine;

[Serializable]
public class NeuralNetworkLayer
{
    public int nInputs;
    public int nOutputs;

    
    public WrapperArray<float>[] weights; // WrapperArray[] can be serialized as opposed to T[,]
    public float[] biases;


    Func<float> WeightInitMethod;
    Func<float> BiasInitMethod;
    Func<float, float> ActivationMethod;

    public NeuralNetworkLayer(int _nInputs, int _nOutputs, Func<float> weightInitMethod, Func<float> biasInitMethod, Func<float, float> activationMethod)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = InitWeights(weightInitMethod);
        biases = InitBiases(biasInitMethod);

        WeightInitMethod = weightInitMethod;
        BiasInitMethod = biasInitMethod;
        ActivationMethod = activationMethod;
    }

    public NeuralNetworkLayer Clone()
    {
        NeuralNetworkLayer clone = new NeuralNetworkLayer(nInputs, nOutputs, WeightInitMethod, BiasInitMethod, ActivationMethod);

        // Copy weights into clone
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

        clone.weights = _weights;

        Array.Copy(biases, clone.biases, biases.Length);

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

    public void Mutate(Func<float, float> WeightMutateMethod, Func<float, float> BiasMutateMethod)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = WeightMutateMethod(weights[i][j]);
            }
        }

        for(int i = 0; i < biases.Length; i++)
        {
            biases[i] = BiasMutateMethod(biases[i]);
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

            outputs[i] = ActivationMethod(value + biases[i]);
        }

        return outputs;
    }

    public (float, float) ComputeTotalWeightAndBias()
    {
        float weightTotal = 0;
        float biasTotal = 0;

        foreach(WrapperArray<float> wrapper in weights)
        {
            foreach(float weight in wrapper)
            {
                weightTotal += weight;
            }
        }
        
        foreach(float bias in biases)
        {
            biasTotal += bias;
        }

        return (weightTotal, biasTotal);
    }
}
