
public class NeuralNetworkLayer
{
    private int nInputs;
    private int nOutputs;

    private float[,] weights;
    private float[] biases;

    public NeuralNetworkLayer(int _nInputs, int _nOutputs)
    {
        nInputs = _nInputs;
        nOutputs = _nOutputs;

        weights = new float[nInputs, nOutputs];
        biases = new float[nOutputs];
    }

    public float[] ComputeOutputs(float[] inputs)
    {
        float[] outputs = new float[nOutputs];

        for(int i = 0; i < nOutputs; i++)
        {
            // TODO calculate the output of the node given the inputs
            outputs[i] = 0.5f;
        }

        return outputs;
    }
}
