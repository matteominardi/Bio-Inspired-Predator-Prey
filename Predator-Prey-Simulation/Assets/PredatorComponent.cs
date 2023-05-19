using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct PredatorTag : IComponentData { }
public struct PredatorComponent : IComponentData
{
    //public Entity prefabPredator;
    //public NeuralNetworkComponent Brain;

    // private Raycast[] inputs;
    public int Lifepoints;
    public float Fitness;
    public bool Alive;

    public float Energy;
    public float Speed;
    public int Generation;
    public float ReproductionFactor;
    //public Raycast Raycast;
    // public DynamicBuffer<IntDataElement> BrainModel;

    // public DynamicBuffer<FloatDataElement> _inputs;
    // public DynamicBuffer<FloatDataElement> _outputs;

    public int _dmg;
    //public int _numRays;
    //public int _fov;
    //public int _viewRange;
}
