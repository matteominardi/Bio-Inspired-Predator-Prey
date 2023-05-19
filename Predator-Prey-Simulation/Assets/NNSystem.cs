using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using System.IO;
using UnityEngine;

//[AlwaysUpdateSystem]

[UpdateAfter(typeof(RaycastUpdateSystem))]
public partial class NNSystem : SystemBase
{
    //NativeArray<int> lengths;
    float time;
    string path;
    protected override void OnStartRunning()
    {
        // int numLayersNN = SceneInitializerECS.BrainStructure().Length;
        // int numInputsNN = SceneInitializerECS.BrainStructure()[0];
        // lengths = new NativeArray<int>(2, Allocator.Persistent);
        // lengths[0] = numLayersNN;
        // lengths[1] = numInputsNN;
        time = UnityEngine.Time.time;
        # if UNITY_EDITOR
        path = "Assets/PretrainedNetworks/";
        # else
        path = Path.Combine(Application.persistentDataPath, "Assets/PretrainedNetworks/");
        # endif
    }
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        
        //NativeArray<int> lengths = this.lengths;
        // int numLayersNN = SceneInitializerECS.BrainStructure().Length;
        // int numInputsNN = SceneInitializerECS.BrainStructure()[0]/2;
        Entities
        .ForEach((ref DynamicBuffer<OutputDataElement> outputs, ref DynamicBuffer<NeuronDataElement> neurons, in DynamicBuffer<DistanceDataElement> distances, in DynamicBuffer<WhoIsThereDataElement> whoIsThere, in DynamicBuffer<LayerDataElement> layers, in DynamicBuffer<WeightDataElement> weights, in DynamicBuffer<BiasDataElement> biases) => {
            // DynamicBuffer<DistanceDataElement> distances = GetBuffer<DistanceDataElement>(entity);
            // DynamicBuffer<WhoIsThereDataElement> whoIsThere = GetBuffer<WhoIsThereDataElement>(entity);
            // int[] layers = GetBuffer<WhoIsThereDataElement>(entity).Reinterpret<int>();
            // DynamicBuffer<NeuronDataElement> neurons = GetBuffer<NeuronDataElement>(entity);
            // DynamicBuffer<WeightDataElement> weights = GetBuffer<WeightDataElement>(entity);
            // DynamicBuffer<BiasDataElement> biases = GetBuffer<BiasDataElement>(entity);
            //int[] layersInt = layers.Reinterpret<int>();
            //FeedForward(distances, whoIsThere, layers, neurons, weights, biases, numLayersNN, numInputsNN);
            int numLayersNN = layers.Length;
            int numInputsNN = distances.Length;

            // feed forward
            DynamicBuffer<float> _distances = distances.Reinterpret<float>();
            DynamicBuffer<float> _whoIsThere = whoIsThere.Reinterpret<float>();
            DynamicBuffer<float> _neurons = neurons.Reinterpret<float>();
            DynamicBuffer<float> _weights = weights.Reinterpret<float>();
            DynamicBuffer<float> _biases = biases.Reinterpret<float>();
            DynamicBuffer<int> _layers = layers.Reinterpret<int>();
            DynamicBuffer<float> _outputs = outputs.Reinterpret<float>();
            _outputs.ResizeUninitialized(2);
            for (int i = 0; i < numInputsNN; i++)
            {
                //_neurons[0][i] = inputs[i];
                _neurons[i*2] = _distances[i];
                //Debug.Log("neuron " + i*2 + " " + _neurons[i*2]);
                //Debug.Log("i " + i);
                
            }
            //Debug.Log("here");

            for (int i = 0; i < numInputsNN; i++)
            {
                //_neurons[0][i] = inputs[i];
                _neurons[i*2+1] = _whoIsThere[i];
                //Debug.Log("neuronWho " + (i*2+1) + " " + _neurons[i*2+1]);
            }

            for (int i = numInputsNN*2; i < _neurons.Length; i++)
            {
                //_neurons[0][i] = inputs[i];
                _neurons[i] = 0;
                //Debug.Log("neuronHidden " + (i) + " " + _neurons[i]);
            }
            //Debug.Log("here2");

            int numNeuronsBefore = 0;//_layers[0];
            int numNeuronsAfter = _layers[0];
            int numWeightsBefore = 0;//_layers[0] * _layers[1];
            

            for (int i = 0; i < numLayersNN - 1; i++)
            {
                for (int j = 0; j < _layers[i]; j++)
                {
                    for (int k = 0; k < _layers[i + 1]; k++)
                    {
                        //neurons[i + 1][k] += weights[i][j][k] * neurons[i][j];
                        //Debug.Log("next " + (numNeuronsAfter + k) + " before " + (numNeuronsBefore));
                        // if (float.IsNaN(_weights[numWeightsBefore + k] * _neurons[numNeuronsBefore]) || float.IsNaN(_weights[numWeightsBefore + k]) || float.IsNaN(_neurons[numNeuronsBefore]) || float.IsNaN(_neurons[numNeuronsAfter + k] + _weights[numWeightsBefore + k] * _neurons[numNeuronsBefore]))
                        //     Debug.Log("weight " + (numWeightsBefore + k) + " " + _weights[numWeightsBefore + k] + " neuronBefore "+ numNeuronsBefore+ ": " + _neurons[numNeuronsBefore] + " result: " + _weights[numWeightsBefore + k] * _neurons[numNeuronsBefore] + " update " + (_neurons[numNeuronsAfter + k] + _weights[numWeightsBefore + k] * _neurons[numNeuronsBefore]));
                        _neurons[numNeuronsAfter + k] += _weights[numWeightsBefore + k] * _neurons[numNeuronsBefore];
                    }
                    numWeightsBefore += _layers[i + 1];
                    numNeuronsBefore += 1;
                }
                //Debug.Log("here3");
                for (int k = 0; k < _layers[i + 1]; k++)
                {
                    // if (float.IsNaN(_neurons[numNeuronsBefore + k]) || float.IsNaN(_biases[numNeuronsBefore + k]) || float.IsNaN(math.tanh(_neurons[numNeuronsBefore + k] + _biases[numNeuronsBefore + k])) || float.IsNaN(_neurons[numNeuronsBefore + k] + _biases[numNeuronsBefore + k]))
                    //     Debug.Log("Update next layer neurons " + (numNeuronsBefore + k) + " " + _neurons[numNeuronsBefore + k] + " bias " + _biases[numNeuronsBefore + k] + " " + math.tanh(_neurons[numNeuronsBefore + k] + _biases[numNeuronsBefore + k]));
                    _neurons[numNeuronsBefore + k] = math.tanh(_neurons[numNeuronsBefore + k] + _biases[numNeuronsBefore + k]);
                }
                numNeuronsAfter += _layers[i + 1];
            }
            //NativeArray<float> outputValues = new NativeArray<float>(_layers[numLayersNN-1], Allocator.Temp);
            for (int i = 0; i < _layers[numLayersNN-1]; i++)
            {
                //Debug.Log("here4" + );
                //
                //Debug.Log("output " + i + " numneusBef+i " + (numNeuronsBefore + i) + " " + _neurons[numNeuronsBefore + i]);
                _outputs[i] = _neurons[numNeuronsBefore + i];
                //Debug.Log("output " + outputValues[i]);
            } 
            //outputValues.Dispose();
            //end feed forward

        }).WithBurst().ScheduleParallel();//.WithoutBurst().Run();//.WithBurst().ScheduleParallel();

        if (UnityEngine.Time.time - time > 10)
        {
            time = UnityEngine.Time.time;
            Entity bestPrey = Entity.Null;
            Entity bestPredator = Entity.Null;
            float bestFitnessPrey = -1000;
            float bestFitnessPredator = -1000;
            Entities
            .WithAll<PreyTag>()
            .ForEach((Entity e, ref PreyComponent preyComponent, in DynamicBuffer<WeightDataElement> weights, in DynamicBuffer<BiasDataElement> biases) => {
                if (preyComponent.Fitness > bestFitnessPrey)
                {
                    bestFitnessPrey = preyComponent.Fitness;
                    bestPrey = e;
                }
            }).Run();

            Entities
            .WithAll<PredatorTag>()
            .ForEach((Entity e, ref PredatorComponent predatorComponent, in DynamicBuffer<WeightDataElement> weights, in DynamicBuffer<BiasDataElement> biases) => {
                if (predatorComponent.Fitness > bestFitnessPredator)
                {
                    bestFitnessPredator = predatorComponent.Fitness;
                    bestPredator = e;
                }
            }).Run();

            if (bestPrey != Entity.Null)
            {
                
                //Debug.Log("best fitness " + bestFitness);
                //Debug.Log("path " + path);
                //Debug.Log("saving at " + path + "PreyBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
                string pathFile = path + "PreyBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                File.Create(pathFile).Close();
                StreamWriter writer = new StreamWriter(pathFile, true);
                DynamicBuffer<float> _biases = GetBuffer<BiasDataElement>(bestPrey, true).Reinterpret<float>();
                DynamicBuffer<float> _weights = GetBuffer<WeightDataElement>(bestPrey, true).Reinterpret<float>();

                for (int i = 0; i < _biases.Length; i++)
                {
                    //for (int j = 0; j < _biases[i].Length; j++)
                    //{
                        writer.WriteLine(_biases[i]);
                    //}
                }

                for (int i = 0; i < _weights.Length; i++)
                {
                    //for (int j = 0; j < _weights[i].Length; j++)
                    //{
                    //    for (int k = 0; k < _weights[i][j].Length; k++)
                    //    {
                            writer.WriteLine(_weights[i]);
                    //    }
                    //}
                }
                writer.Write((char)26); // Append EOF character
                writer.Close();
            }

            if (bestPredator != Entity.Null)
            {
                
                //Debug.Log("best fitness " + bestFitness);
                //Debug.Log("path " + path);
                //Debug.Log("saving at " + path + "PredatorBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
                string pathFile = path + "PredatorBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                File.Create(pathFile).Close();
                StreamWriter writer = new StreamWriter(pathFile, true);
                DynamicBuffer<float> _biases = GetBuffer<BiasDataElement>(bestPredator, true).Reinterpret<float>();
                DynamicBuffer<float> _weights = GetBuffer<WeightDataElement>(bestPredator, true).Reinterpret<float>();

                for (int i = 0; i < _biases.Length; i++)
                {
                    //for (int j = 0; j < _biases[i].Length; j++)
                    //{
                        writer.WriteLine(_biases[i]);
                    //}
                }

                for (int i = 0; i < _weights.Length; i++)
                {
                    //for (int j = 0; j < _weights[i].Length; j++)
                    //{
                    //    for (int k = 0; k < _weights[i][j].Length; k++)
                    //    {
                            writer.WriteLine(_weights[i]);
                    //    }
                    //}
                }
                writer.Write((char)26); // Append EOF character
                writer.Close();
            }
        }
    }

    private float Activate(float value)
    {
        return (float)math.tanh(value);
    }

    public NativeArray<float> FeedForward(DynamicBuffer<DistanceDataElement> _distances, DynamicBuffer<WhoIsThereDataElement> _whoIsThere, DynamicBuffer<LayerDataElement> _layers, DynamicBuffer<NeuronDataElement> _neurons, DynamicBuffer<WeightDataElement> _weights, DynamicBuffer<BiasDataElement> _biases, int numLayersNN, int numInputsNN)
    {
        // int numLayersNN = lengths[0];
        // int numInputsNN = lengths[1];
        DynamicBuffer<float> distances = _distances.Reinterpret<float>();
        DynamicBuffer<float> whoIsThere = _whoIsThere.Reinterpret<float>();
        DynamicBuffer<float> neurons = _neurons.Reinterpret<float>();
        DynamicBuffer<float> weights = _weights.Reinterpret<float>();
        DynamicBuffer<float> biases = _biases.Reinterpret<float>();
        DynamicBuffer<int> layers = _layers.Reinterpret<int>();
        for (int i = 0; i < numInputsNN; i+=2)
        {
            //_neurons[0][i] = inputs[i];
            neurons[i] = distances[i];
        }

        for (int i = 1; i < numInputsNN; i+=2)
        {
            //_neurons[0][i] = inputs[i];
            neurons[i] = whoIsThere[i];
        }

        int numNeuronsBefore = layers[0];
        int numWeightsBefore = layers[0] * layers[1];
        

        for (int i = 0; i < numLayersNN - 1; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                for (int k = 0; k < layers[i + 1]; k++)
                {
                    //neurons[i + 1][k] += weights[i][j][k] * neurons[i][j];
                    neurons[numNeuronsBefore + k] += weights[numWeightsBefore + k] * neurons[i * layers[i] + j];
                }
                numWeightsBefore += layers[i + 1];
                numNeuronsBefore += 1;
            }
            for (int k = 0; k < layers[i + 1]; k++)
            {
                neurons[numNeuronsBefore + k] = Activate(neurons[numNeuronsBefore + k] + biases[numNeuronsBefore + k]);
            }
        }
        NativeArray<float> outputValues = new NativeArray<float>(layers[numLayersNN], Allocator.Temp);
        for (int i = 0; i < layers[numLayersNN]; i++)
        {
            outputValues[i] = neurons[numNeuronsBefore + i];
        } 
        return outputValues;
        //return _neurons[_neurons.Length - 1];
    }

    /* 
    chance is the chance of mutation, val is the standard deviation of the mutation values, chance should be between 0 and 10
     */
    // public void Mutate(float mutationRate, float mutationAmount)
    // {
    //     for (int i = 0; i < _biases.Length; i++)
    //     {
    //         for (int j = 0; j < _biases[i].Length; j++)
    //         {
    //             _biases[i][j] = (UnityEngine.Random.Range(0f, 1f) <= mutationRate) ? _biases[i][j] += UnityEngine.Random.Range(-mutationAmount, mutationAmount) : _biases[i][j];
    //         }
    //     }

    //     for (int i = 0; i < _weights.Length; i++)
    //     {
    //         for (int j = 0; j < _weights[i].Length; j++)
    //         {
    //             for (int k = 0; k < _weights[i][j].Length; k++)
    //             {
    //                 _weights[i][j][k] = (UnityEngine.Random.Range(0f, 1f) <= mutationRate) ? _weights[i][j][k] += UnityEngine.Random.Range(-mutationAmount, mutationAmount) : _weights[i][j][k];

    //             }
    //         }
    //     }
    // }


    // /*
    // takes a NN in input and edits it in order to make it a copy of the current one
    // */
    // public NeuralNetwork Copy(NeuralNetwork nn)
    // {
    //     for (int i = 0; i < _biases.Length; i++)
    //     {
    //         for (int j = 0; j < _biases[i].Length; j++)
    //         {
    //             nn._biases[i][j] = _biases[i][j];
    //         }
    //     }
    //     for (int i = 0; i < _weights.Length; i++)
    //     {
    //         for (int j = 0; j < _weights[i].Length; j++)
    //         {
    //             for (int k = 0; k < _weights[i][j].Length; k++)
    //             {
    //                 nn._weights[i][j][k] = _weights[i][j][k];
    //             }
    //         }
    //     }
    //     return nn;
    // }

    // /*
    // function used to load the _weights and _biases of the network from a file, so that it can be used for the next generations
    // */
    // public void Load(string path)
    // {
    //     StreamReader tr = new StreamReader(path);
    //     int NumberOfLines = (int)new FileInfo(path).Length;
    //     List<string> ListLines = new List<string>();
    //     int index = 0;


    //     while (!tr.EndOfStream)
    //     {
    //         ListLines.Add(tr.ReadLine());
    //     }
    //     //string[] ArrayLines = ListLines.ToArray();
    //     Debug.Log("Number of lines: " + ListLines.Count);
    //     // for (int i = 0; i < NumberOfLines; i++)
    //     // {

    //     //     ListLines[i] = tr.ReadLine();

    //     //     Debug.Log("LINE " + i + " :" + ListLines[i]);
    //     // }

    //     tr.Close();

    //     if (new FileInfo(path).Length > 0)
    //     {
    //         for (int i = 0; i < _biases.Length; i++)
    //         {
    //             for (int j = 0; j < _biases[i].Length; j++)
    //             {
    //                 _biases[i][j] = float.Parse(ListLines[index]);
    //                 index++;
    //             }
    //         }

    //         for (int i = 0; i < _weights.Length; i++)
    //         {
    //             for (int j = 0; j < _weights[i].Length; j++)
    //             {
    //                 for (int k = 0; k < _weights[i][j].Length; k++)
    //                 {
    //                     _weights[i][j][k] = float.Parse(ListLines[index]);
    //                     index++;
    //                 }
    //             }
    //         }
    //     }
    // }

    // /*
    // function used to save the _weights and _biases of the network to a file, so that it can be used for the next generations
    // */
    // public void Save(string path)
    // {
    //     File.Create(path).Close();
    //     StreamWriter writer = new StreamWriter(path, true);

    //     for (int i = 0; i < _biases.Length; i++)
    //     {
    //         for (int j = 0; j < _biases[i].Length; j++)
    //         {
    //             writer.WriteLine(_biases[i][j]);
    //         }
    //     }

    //     for (int i = 0; i < _weights.Length; i++)
    //     {
    //         for (int j = 0; j < _weights[i].Length; j++)
    //         {
    //             for (int k = 0; k < _weights[i][j].Length; k++)
    //             {
    //                 writer.WriteLine(_weights[i][j][k]);
    //             }
    //         }
    //     }
    //     writer.Write((char)26); // Append EOF character
    //     writer.Close();
    // }
}
