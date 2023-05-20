using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

// public interface IComponent
// {
//     Transform transform { get; }
//     GameObject gameObject { get; }
// }

public struct IntDataElement : IBufferElementData
{
    public int Value;
}

[InternalBufferCapacity(2)]
public struct OutputDataElement : IBufferElementData
{
    public float Value;
}


public struct FloatDataElement : IBufferElementData
{
    public int Value;
}


public interface ISelectableEntity
{
    int Lifepoints { get; set; }
    float Fitness { get; set; }
    bool Alive { get; set; }
    float Energy { get; set; }
    float Speed { get; set; }
    public int Generation { get; set; }
    public Raycast Raycast { get; set; }
    public DynamicBuffer<IntDataElement> BrainModel { get; set; }
    public NeuralNetworkComponent Brain { get; set; }

}

public struct PreyTag : IComponentData { }

//[GenerateAuthoringComponent]
public struct PreyComponent : IComponentData//, ISelectableEntity 
{
    //public Entity prefabPrey;
    //private readonly object _lockPreys = new object();
    //public NeuralNetworkComponent Brain;
    // private Raycast[] inputs;
    public int Lifepoints;
    public float Fitness;
    public bool Alive;

    public float Energy;
    public float Speed;
    public int Generation;
    //public Raycast Raycast
    //public DynamicBuffer<IntDataElement> BrainModel;



    public bool _energyExhausted;

    public float _age;
    //public DynamicBuffer<FloatDataElement> _inputs;
    //public DynamicBuffer<FloatDataElement> _outputs;
    //public bool guard = true;
    public int _counterReproduction;
    public int _dmg;
    public float _timerStopCollision;
    //public int _numRays;
    //public int _fov;
    //public int _viewRange;
}
