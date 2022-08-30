
public class NeuralNetwork
{
    private int nLayers;
    private int[] layerSizes;
    private NeuralNetworkLayer[] layers;

    public NeuralNetwork(int[] _layerSizes)
    {
        layerSizes = _layerSizes;
        nLayers = layerSizes.Length;

        // Last layer does not need to be stored, since it is only a output layer
        layers = new NeuralNetworkLayer[nLayers - 1];
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new NeuralNetworkLayer(layerSizes[i], layerSizes[i + 1]);
        }
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
}
