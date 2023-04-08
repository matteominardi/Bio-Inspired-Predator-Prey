using System.Collections.Generic;
using System;
using System.IO;

public class NeuralNetwork
{
    private int[] Layers;           // array with the number of Neurons in in each layer, i.e. [5,4,3]
    private float[][] Neurons;      // array with the Neurons in each layer, i.e. [[0.1, 0.2, 0.3, 0.4, 0.5], [0.1, 0.2, 0.3, 0.4], [0.1, 0.2, 0.3]]
    private float[][] Biases;       // array with the Biases in each layer, same number of Neurons as in the layer
    private float[][][] Weights;    // array with the outgoing Weights between each layer , i.e. [[[layer1], [layer1], [layer1], [layer1], [layer1]], [[layer2], [layer2], [layer2], [layer2]], [[layer3], [layer3], [layer3]]
    private int[] Activations;
    public double Fitness = 0;

    public NeuralNetwork(int[] Layers)
    {
        this.Layers = new int[Layers.Length];

        for (int i = 0; i < Layers.Length; i++)
        {
            this.Layers[i] = Layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();
    }

    /*
    creates a new array with the same number of Neurons as in the Layers array, to which it will be assigned a value later
    */
    private void InitNeurons()
    {
        List<float[]> NeuronsList = new List<float[]>();

        for (int i = 0; i < Layers.Length; i++)
        {
            NeuronsList.Add(new float[Layers[i]]);
        }

        Neurons = NeuronsList.ToArray();
    }

    /*
    assigns a random value to each bias for each neuron between -0.5 and 0.5
    */
    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();

        for (int i = 0; i < Layers.Length; i++)
        {
            float[] bias = new float[Layers[i]];

            for (int j = 0; j < Layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }

            biasList.Add(bias);
        }

        Biases = biasList.ToArray();
    }

    /*
    for each node, computes the number of incoming connections, and assigns a random value to each of them between -0.5 and 0.5
    */
    private void InitWeights()
    {
        List<float[][]> WeightsList = new List<float[][]>();

        for (int i = 1; i < Layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int NeuronsInPreviousLayer = Layers[i - 1];

            for (int j = 0; j < Neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[NeuronsInPreviousLayer];

                for (int k = 0; k < NeuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            WeightsList.Add(layerWeightsList.ToArray());
        }

        Weights = WeightsList.ToArray();
    }

    /*
    activation function used to calculate the output of a neuron, using the hyperbolic tangent function
    */
    private float Activate(float value)
    {
        return (float)Math.Tanh(value);
    }

    /*
    performs the weighted sum of the inputs and the Biases, and then applies the activation function to the result of each neuron in the network
    returns the output layer of the network
    */
    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            Neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < Layers.Length; i++)
        {
            int layer = i - 1;

            for (int j = 0; j < Neurons[i].Length; j++)
            {
                float value = 0f;

                for (int k = 0; k < Neurons[i - 1].Length; k++)
                {
                    value += Weights[i - 1][j][k] * Neurons[i - 1][k];
                }

                Neurons[i][j] = Activate(value + Biases[i][j]);
            }
        }
        return Neurons[Neurons.Length - 1];
    }

    /* 
    chance is the chance of mutation, val is the standard deviation of the mutation values, chance should be between 0 and 10
     */
    public void Mutate(int chance, float val)
    {
        for (int i = 0; i < Biases.Length; i++)
        {
            for (int j = 0; j < Biases[i].Length; j++)
            {
                Biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? Biases[i][j] += UnityEngine.Random.Range(-val, val) : Biases[i][j];
            }
        }

        for (int i = 0; i < Weights.Length; i++)
        {
            for (int j = 0; j < Weights[i].Length; j++)
            {
                for (int k = 0; k < Weights[i][j].Length; k++)
                {
                    Weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? Weights[i][j][k] += UnityEngine.Random.Range(-val, val) : Weights[i][j][k];

                }
            }
        }
    }

    /*
    function used to understand which network between two candidates is better, based on their Fitness 
    */
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null || Fitness > other.Fitness)
            return 1;
        else if (Fitness < other.Fitness)
            return -1;
        else
            return 0;
    }

    /*
    takes a NN in input and edits it in order to make it a copy of the current one
    */
    public NeuralNetwork Copy(NeuralNetwork nn)
    {
        for (int i = 0; i < Biases.Length; i++)
        {
            for (int j = 0; j < Biases[i].Length; j++)
            {
                nn.Biases[i][j] = Biases[i][j];
            }
        }
        for (int i = 0; i < Weights.Length; i++)
        {
            for (int j = 0; j < Weights[i].Length; j++)
            {
                for (int k = 0; k < Weights[i][j].Length; k++)
                {
                    nn.Weights[i][j][k] = Weights[i][j][k];
                }
            }
        }
        return nn;
    }

    /*
    function used to load the Weights and Biases of the network from a file, so that it can be used for the next generations
    */
    public void Load(string path)
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;

        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }

        tr.Close();

        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < Biases.Length; i++)
            {
                for (int j = 0; j < Biases[i].Length; j++)
                {
                    Biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < Weights.Length; i++)
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }
    }

    /*
    function used to save the Weights and Biases of the network to a file, so that it can be used for the next generations
    */
    public void Save(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < Biases.Length; i++)
        {
            for (int j = 0; j < Biases[i].Length; j++)
            {
                writer.WriteLine(Biases[i][j]);
            }
        }

        for (int i = 0; i < Weights.Length; i++)
        {
            for (int j = 0; j < Weights[i].Length; j++)
            {
                for (int k = 0; k < Weights[i][j].Length; k++)
                {
                    writer.WriteLine(Weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}