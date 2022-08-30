using System;

public class NeuralNetworkLayer
{
    private int nInputs;
    private int nOutputs;

    private float[,] weights;
    private float[] biases;

    public NeuralNetworkLayer(int _nInputs, int _nOutputs, Func<float> weightInitMethod, Func<float> biasInitMethod)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = InitWeights(weightInitMethod);
        biases = InitBiases(biasInitMethod);
    }

    private float[,] InitWeights(Func<float> initMethod)
    {
        float[,] _weights = new float[nInputs, nOutputs];

        for(int i = 0; i < _weights.GetLength(0); i++)
        {
            for(int j = 0; j < _weights.GetLength(1); j++)
            {
                _weights[i, j] = initMethod();
            }
        }

        return _weights;
    }

    private float[] InitBiases(Func<float> initMethod)
    {
        float[] _biases = new float[nOutputs];

        for(int i = 0; i < _biases.Length; i++)
        {
            _biases[i] = initMethod();
        }

        return _biases;
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

            outputs[i] = value + biases[i];
        }

        return outputs;
    }
}
