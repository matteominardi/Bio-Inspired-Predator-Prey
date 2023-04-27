using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class NeuralNetwork
{
    private int[] _layers;           // array with the number of _neurons in in each layer, i.e. [5,4,3]
    private float[][] _neurons;      // array with the _neurons in each layer, i.e. [[0.1, 0.2, 0.3, 0.4, 0.5], [0.1, 0.2, 0.3, 0.4], [0.1, 0.2, 0.3]]
    private float[][] _biases;       // array with the _biases in each layer, same number of _neurons as in the layer
    private float[][][] _weights;    // array with the outgoing _weights between each layer , i.e. [[[layer1], [layer1], [layer1], [layer1], [layer1]], [[layer2], [layer2], [layer2], [layer2]], [[layer3], [layer3], [layer3]]
    private int[] _activations;

    public float this[int i, int j, int k]
    {
        get { return _weights[i][j][k]; }
    }

    public NeuralNetwork(int[] _layers)
    {
        this._layers = new int[_layers.Length];

        for (int i = 0; i < _layers.Length; i++)
        {
            this._layers[i] = _layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();
    }

    /*
    creates a new array with the same number of _neurons as in the _layers array, to which it will be assigned a value later
    */
    private void InitNeurons()
    {
        List<float[]> NeuronsList = new List<float[]>();

        for (int i = 0; i < _layers.Length; i++)
        {
            NeuronsList.Add(new float[_layers[i]]);
        }

        _neurons = NeuronsList.ToArray();
    }

    /*
    assigns a random value to each bias for each neuron between -0.5 and 0.5
    */
    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();

        for (int i = 0; i < _layers.Length; i++)
        {
            float[] bias = new float[_layers[i]];

            for (int j = 0; j < _layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }

            biasList.Add(bias);
        }

        _biases = biasList.ToArray();
    }

    /*
    for each node, computes the number of incoming connections, and assigns a random value to each of them between -0.5 and 0.5
    */
    // private void InitWeights()
    // {
    //     List<float[][]> WeightsList = new List<float[][]>();

    //     for (int i = 0; i < _layers.Length; i++)
    //     {
    //         List<float[]> layerWeightsList = new List<float[]>();
    //         int NeuronsInPreviousLayer = _layers[i - 1];
            

    //         for (int j = 0; j < _neurons[i].Length; j++)
    //         {
    //             float[] neuronWeights = new float[NeuronsInPreviousLayer];

    //             for (int k = 0; k < NeuronsInPreviousLayer; k++)
    //             {
    //                 neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
    //             }

    //             layerWeightsList.Add(neuronWeights);
    //         }

    //         WeightsList.Add(layerWeightsList.ToArray());
    //     }

    //     _weights = WeightsList.ToArray();
    // }
    private void InitWeights()
    {
        List<float[][]> WeightsList = new List<float[][]>();

        for (int i = 0; i < _layers.Length - 1; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInNextLayer = _layers[i + 1];            

            for (int j = 0; j < _neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInNextLayer];

                for (int k = 0; k < neuronsInNextLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.01f, 0.01f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            WeightsList.Add(layerWeightsList.ToArray());
        }

        _weights = WeightsList.ToArray();
    }

    /*
    activation function used to calculate the output of a neuron, using the hyperbolic tangent function
    */
    private float Activate(float value)
    {
        return (float)Math.Tanh(value);
    }

    /*
    performs the weighted sum of the inputs and the _biases, and then applies the activation function to the result of each neuron in the network
    returns the output layer of the network
    */
    // public float[] FeedForward(float[] inputs)
    // {
    //     for (int i = 0; i < inputs.Length; i++)
    //     {
    //         _neurons[0][i] = inputs[i];
    //     }

    //     for (int i = 1; i < _layers.Length; i++)
    //     {
    //         int layer = i - 1;

    //         for (int j = 0; j < _neurons[i].Length; j++)
    //         {
    //             float value = 0f;

    //             for (int k = 0; k < _neurons[i - 1].Length; k++)
    //             {
    //                 value += _weights[i - 1][j][k] * _neurons[i - 1][k];
    //             }

    //             _neurons[i][j] = Activate(value + _biases[i][j]);
    //         }
    //     }
    //     return _neurons[_neurons.Length - 1];
    // }
    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            _neurons[0][i] = inputs[i];
        }

        for (int i = 0; i < _layers.Length - 1; i++)
        {
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                for (int k = 0; k < _neurons[i + 1].Length; k++)
                {
                    _neurons[i + 1][k] += _weights[i][j][k] * _neurons[i][j];
                }
            }
            for (int k = 0; k < _neurons[i + 1].Length; k++)
            {
                _neurons[i + 1][k] = Activate(_neurons[i + 1][k] + _biases[i + 1][k]);
            }
        }
        return _neurons[_neurons.Length - 1];
    }

    /* 
    chance is the chance of mutation, val is the standard deviation of the mutation values, chance should be between 0 and 10
     */
    public void Mutate(int chance, float val)
    {
        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                _biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? _biases[i][j] += UnityEngine.Random.Range(-val, val) : _biases[i][j];
            }
        }

        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    _weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? _weights[i][j][k] += UnityEngine.Random.Range(-val, val) : _weights[i][j][k];

                }
            }
        }
    }


    /*
    takes a NN in input and edits it in order to make it a copy of the current one
    */
    public NeuralNetwork Copy(NeuralNetwork nn)
    {
        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                nn._biases[i][j] = _biases[i][j];
            }
        }
        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    nn._weights[i][j][k] = _weights[i][j][k];
                }
            }
        }
        return nn;
    }

    /*
    function used to load the _weights and _biases of the network from a file, so that it can be used for the next generations
    */
    public void Load(string path)
    {
        StreamReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        List<string> ListLines = new List<string>();
        int index = 0;
        
        
        while (!tr.EndOfStream)
        {
            ListLines.Add(tr.ReadLine());
        }
        //string[] ArrayLines = ListLines.ToArray();
        Debug.Log("Number of lines: " + ListLines.Count);
        // for (int i = 0; i < NumberOfLines; i++)
        // {
            
        //     ListLines[i] = tr.ReadLine();
            
        //     Debug.Log("LINE " + i + " :" + ListLines[i]);
        // }

        tr.Close();

        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < _biases.Length; i++)
            {
                for (int j = 0; j < _biases[i].Length; j++)
                {
                    _biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    for (int k = 0; k < _weights[i][j].Length; k++)
                    {
                        _weights[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }
    }

    /*
    function used to save the _weights and _biases of the network to a file, so that it can be used for the next generations
    */
    public void Save(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < _biases.Length; i++)
        {
            for (int j = 0; j < _biases[i].Length; j++)
            {
                writer.WriteLine(_biases[i][j]);
            }
        }

        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    writer.WriteLine(_weights[i][j][k]);
                }
            }
        }
        writer.Write((char)26); // Append EOF character
        writer.Close();
    }
}