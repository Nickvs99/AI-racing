using System;

[Serializable]
public class NeuralNetworkLayer
{
    private int nInputs;
    private int nOutputs;

    private float[,] weights;
    private float[] biases;

    Func<float, float> ActivationMethod;

    public NeuralNetworkLayer(int _nInputs, int _nOutputs, Func<float> weightInitMethod, Func<float> biasInitMethod, Func<float, float> activationMethod)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = InitWeights(weightInitMethod);
        biases = InitBiases(biasInitMethod);

        ActivationMethod = activationMethod;
    }

    private float[,] InitWeights(Func<float> InitMethod)
    {
        float[,] _weights = new float[nInputs, nOutputs];

        for(int i = 0; i < _weights.GetLength(0); i++)
        {
            for(int j = 0; j < _weights.GetLength(1); j++)
            {
                _weights[i, j] = InitMethod();
            }
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
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                weights[i, j] = WeightMutateMethod(weights[i, j]);
            }
        }

        for(int i = 0; i < biases.Length; i++)
        {
            biases[i] = BiasMutateMethod(biases[i]);
        }
    }
    
    public float[] ComputeOutputs(float[] inputs)
    {
        float[] outputs = new float[nOutputs];

        for(int i = 0; i < nOutputs; i++)
        {
            float value = 0;
            for(int j = 0; j < nInputs; j++)
            {
                value += weights[j, i] * inputs[j];
            }

            outputs[i] = ActivationMethod(value + biases[i]);
        }

        return outputs;
    }
}
