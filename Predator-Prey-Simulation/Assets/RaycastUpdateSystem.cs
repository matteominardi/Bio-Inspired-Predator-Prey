using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;
using Unity.Rendering;
using Unity.Profiling;


public partial class RaycastUpdateSystem : SystemBase
{
    private Mesh meshAgent;
    private UnityEngine.Material materialPrey;
    private Entity _prefabPrey;
    private Entity _prefabPredator;
    private Unity.Mathematics.Random _random;
    private LayerMask _layerMask;
    BuildPhysicsWorld buildPhysicsWorld;
    CollisionWorld collisionWorld;
    EntityArchetype archetypePrey;
    // static readonly ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("RaycastSystemJob");
    // static readonly ProfilerMarker s_MathPerfMarker = new ProfilerMarker("RaycastMathSystemJob");
    // static readonly ProfilerMarker s_RaycastShootPerfMarker = new ProfilerMarker("RaycastShootSystemJob");
    // static readonly ProfilerMarker s_RaycastDrawRayMarker = new ProfilerMarker("RaycastDrawRaySystemJob");
    protected override void OnStartRunning()
    {
        //_prefabPrey = GetSingleton<PreyComponent>().prefabPrey;
        //_prefabPredator = GetSingleton<PredatorComponent>().prefabPredator;
        // meshAgent = SceneInitializerECS.MeshAgent;
        // materialPrey = SceneInitializerECS.MaterialPrey;
        // _random.InitState(120);// = new Unity.Mathematics.Random(1);
        // _layerMask = ~((1 << 6) | (1 << 5));
        // buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        // collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

        // archetypePrey = EntityManager.CreateArchetype(
        //     typeof(PreyTag),
        //     typeof(Translation),
        //     typeof(Rotation),
        //     typeof(PreyComponent),
        //     typeof(RaycastComponent),
        //     typeof(DistanceDataElement),
        //     typeof(WhoIsThereDataElement),
        //     typeof(LayerDataElement),
        //     typeof(NeuronDataElement),
        //     typeof(WeightDataElement),
        //     typeof(BiasDataElement),
        //     typeof(PhysicsCollider),
        //     typeof(PhysicsVelocity),
        //     //typeof(PhysicsMass),
        //     //typeof(PhysicsShape),
        //     typeof(LocalToWorld),
        //     typeof(RenderMesh),
        //     typeof(RenderBounds),
        //     typeof(PhysicsMass),
        //     typeof(PhysicsWorldIndex),
        //     typeof(PhysicsDamping),
        //     typeof(PhysicsGravityFactor)
        //     //typeof()
        // );


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
        //Dependency.Complete();
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     for (int i = 0; i < SceneInitializerECS.NUMPREY; i++)
        //     { 
        //         var entity = EntityManager.CreateEntity(archetypePrey);
        //         float3 randomPos = new float3(UnityEngine.Random.Range(-SceneInitializerECS.mapSize * 5 + 2, SceneInitializerECS.mapSize * 5 - 2), UnityEngine.Random.Range(-SceneInitializerECS.mapSize * 5 + 2, SceneInitializerECS.mapSize * 5 - 2), 0f);
        //         //EntityManager.SetComponentData(entity, new Translation { Value = new float3(_random.NextFloat(-3f, 3f), _random.NextFloat(-3f, 3f), 0f) });
        //         EntityManager.SetComponentData(entity, new Translation { Value = randomPos });
        //         EntityManager.SetComponentData(entity, new Rotation { Value = quaternion.Euler(0f, 0f, math.radians(0f)) }); //_random.NextFloat(0f, 0f) math.radians(180f)
        //         EntityManager.SetComponentData(entity, new PreyComponent() { 
        //             //Brain = new NeuralNetworkComponent(),
        //             // private Raycast[] inputs;
        //             Lifepoints = 100,
        //             Fitness = 0f,
        //             Alive = true,

        //             Energy = 100f,
        //             Speed = 2f,
        //             Generation = 0,
        //             //public Raycast Raycast
        //             //public DynamicBuffer<IntDataElement> BrainModel;



        //             _energyExhausted = false,

        //             _age = (float)Time.ElapsedTime,
        //             //public DynamicBuffer<FloatDataElement> _inputs;
        //             //public DynamicBuffer<FloatDataElement> _outputs;
        //             //public bool guard = true;
        //             _counterReproduction = 0,
        //             _dmg = 8
        //             // _numRays;
        //             // public int _fov;
        //             // public int _viewRange;
        //         });

                

        //         EntityManager.SetComponentData(entity, new RaycastComponent() { 
        //             //physicsWorld = buildPhysicsWorld.PhysicsWorld,
        //             _numberOfRays = SceneInitializerECS.numRaysPreys,
        //             _fov = SceneInitializerECS.fovPreys,
        //             _viewRange = SceneInitializerECS.viewRangePreys,
        //             toggleShowRays = false
        //         });
        //         EntityManager.AddBuffer<DistanceDataElement>(entity);
        //         EntityManager.AddBuffer<WhoIsThereDataElement>(entity);
        //         EntityManager.AddBuffer<LayerDataElement>(entity);
        //         EntityManager.AddBuffer<NeuronDataElement>(entity);
        //         EntityManager.AddBuffer<WeightDataElement>(entity);
        //         EntityManager.AddBuffer<BiasDataElement>(entity);
        //         EntityManager.AddBuffer<OutputDataElement>(entity);

        //         EntityManager.GetBuffer<DistanceDataElement>(entity).ResizeUninitialized(SceneInitializerECS.numRaysPreys);
        //         EntityManager.GetBuffer<WhoIsThereDataElement>(entity).ResizeUninitialized(SceneInitializerECS.numRaysPreys);
        //         int[] brainStructure = SceneInitializerECS.BrainStructure();
        //         EntityManager.GetBuffer<LayerDataElement>(entity).Reinterpret<int>().CopyFrom(brainStructure);
        //         //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
        //         int numNeurons = 0;
        //         for (int j = 0; j < brainStructure.Length; j++)
        //         {
        //             numNeurons += brainStructure[j];
        //         }
        //         EntityManager.GetBuffer<NeuronDataElement>(entity).ResizeUninitialized(numNeurons);
        //         DynamicBuffer<WeightDataElement> weights = EntityManager.GetBuffer<WeightDataElement>(entity);
        //         DynamicBuffer<BiasDataElement> biases = EntityManager.GetBuffer<BiasDataElement>(entity);

        //         //InitNeurons(neurons, brainStructure);
        //         InitWeights(weights, brainStructure);
        //         InitBiases(biases, brainStructure);

        //         EntityManager.SetSharedComponentData(entity, new RenderMesh() { 
        //             mesh = meshAgent,
        //             material = materialPrey,
        //             layerMask = 1
        //         });

        //         EntityManager.SetComponentData(entity, new PhysicsMass {
        //             InverseMass = 1f,
        //             InverseInertia = new float3(1f, 1f, 1f),
        //             Transform = RigidTransform.identity
        //         });

        //         EntityManager.SetComponentData(entity, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
        //             Center = new float3(0f, 0f, 0f),
        //             Radius = 0.5f
        //         }, Unity.Physics.CollisionFilter.Default) });

        //         EntityManager.SetComponentData(entity, new PhysicsDamping()
        //         {
        //             Linear = 4f,
        //             Angular = 4f
        //         });

        //         EntityManager.SetComponentData(entity, new PhysicsGravityFactor()
        //         {
        //             Value = 0f
        //         });
                
        //         // var physicsMass = PhysicsMass.CreateDynamic(new MassProperties() , 1f);    
        //         // EntityManager.SetComponentData(entity, physicsMass);
                


                
        //         // var entity = EntityManager.Instantiate(_prefabPrey);
        //         // EntityManager.SetComponentData(entity, new Translation { Value = new float3(_random.NextFloat(-3f, 3f), _random.NextFloat(-3f, 3f), 0f) });
        //         // EntityManager.SetComponentData(entity, new Rotation { Value = quaternion.Euler(0f, 0f, math.radians(180f)) }); //_random.NextFloat(0f, 0f) math.radians(180f)
        //         // EntityManager.AddComponentData<RaycastComponent>(entity, new RaycastComponent() { 
        //         //     //physicsWorld = buildPhysicsWorld.PhysicsWorld,
        //         //     _numberOfRays = SceneInitializerECS.numRaysPreys,
        //         //     _fov = SceneInitializerECS.fovPreys,
        //         //     _viewRange = SceneInitializerECS.viewRangePreys,
        //         //     toggleShowRays = false
        //         // });
        //         // EntityManager.AddBuffer<DistanceDataElement>(entity);
        //         // EntityManager.AddBuffer<WhoIsThereDataElement>(entity);

        //         // EntityManager.GetBuffer<DistanceDataElement>(entity).Reinterpret<float>().ResizeUninitialized(SceneInitializerECS.numRaysPreys);
        //         // EntityManager.GetBuffer<WhoIsThereDataElement>(entity).Reinterpret<float>().ResizeUninitialized(SceneInitializerECS.numRaysPreys);

        //         // EntityManager.AddComponentData<PreyComponent>(entity, new PreyComponent(){
        //         //     Brain = new NeuralNetworkComponent(),
        //         //     // private Raycast[] inputs;
        //         //     Lifepoints = 100,
        //         //     Fitness = 0f,
        //         //     Alive = true,

        //         //     Energy = 100f,
        //         //     Speed = 2f,
        //         //     Generation = 0,
        //         //     //public Raycast Raycast
        //         //     //public DynamicBuffer<IntDataElement> BrainModel;



        //         //     _energyExhausted = false,

        //         //     _age = (float)Time.ElapsedTime,
        //         //     //public DynamicBuffer<FloatDataElement> _inputs;
        //         //     //public DynamicBuffer<FloatDataElement> _outputs;
        //         //     //public bool guard = true;
        //         //     _counterReproduction = 0,
        //         //     _dmg = 8
        //         //     // _numRays;
        //         //     // public int _fov;
        //         //     // public int _viewRange;
        //         // });
        //     }
            
        // }
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        NativeArray<RigidBody> buildPhysicsWorldBodies = buildPhysicsWorld.PhysicsWorld.Bodies;
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        float deltaTime = Time.DeltaTime;
        //EntityManager entityManager = EntityManager;
        
        Entities
        //.WithName("RaycastSystemJob")
        .WithReadOnly(collisionWorld)
        //.WithReadOnly(buildPhysicsWorldBodies)
        //.WithReadOnly(entityManager)
        .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref RaycastComponent raycastComponent, ref DynamicBuffer<DistanceDataElement> distances, ref DynamicBuffer<WhoIsThereDataElement> whosThere) => {
        //     // Implement the work to perform for each entity here.
        //     // You should only access data that is local or that is a
        //     // field on this job. Note that the 'rotation' parameter is
        //     // marked as 'in', which means it cannot be modified,
        //     // but allows this job to run in parallel with other jobs
        //     // that want to read Rotation component data.
        //     // For example,
        //     //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
            //s_PreparePerfMarker.Begin();

            var pos = translation.Value;
            //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            for (int i = 0; i < raycastComponent._numberOfRays; i++)
            {

                //s_MathPerfMarker.Begin();
                float step = (float)raycastComponent._fov / (float)(raycastComponent._numberOfRays - 1);
                float3 up = math.up();
                float3 rotatedUp = math.rotate(rotation.Value, up);
                float magnitude = rotatedUp.x * rotatedUp.x + rotatedUp.y * rotatedUp.y + rotatedUp.z * rotatedUp.z;
                float cosineAngle = (math.acos(math.clamp(math.dot(rotatedUp, up) / (magnitude), -1.0f, 1.0f)));
                float angle = (float)((float)i * step  + cosineAngle) % 360.0f - (float)raycastComponent._fov * 0.5f;
                float3 dir = math.mul(quaternion.AxisAngle(math.forward(), math.radians(angle)), rotatedUp);
                //Debug.Log("step " + step + " cosineanlge "+ cosineAngle +" angle " + angle + " dir " + dir + " rotatedUp " + rotatedUp + " magnitude " + magnitude + " cosineAngle " + cosineAngle);
                
                //Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();
                //NativeList<Unity.Physics.RaycastHit> raycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
                float3 end = pos + math.normalize(dir) * raycastComponent._viewRange;
                // if (float.IsNaN(dir.x))
                //     Debug.Log("dot " + math.dot(rotatedUp, up) + " division " + math.dot(rotatedUp, up)/magnitude + " magnitude " + magnitude + " cosineAngle " + cosineAngle);
                    //Debug.Log("angle " + angle + " rad " + math.radians(angle) + " dir " + dir + " rotatedUp " + rotatedUp + " magnitude " + magnitude + " cosineAngle " + cosineAngle);
                // if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) || float.IsNaN(dir.x) || float.IsNaN(dir.y) || float.IsNaN(dir.z) || float.IsNaN(end.x) || float.IsNaN(end.y) || float.IsNaN(end.z))
                //     Debug.Log("pos: " + pos.x + "length ray " + math.length(math.normalize(dir) * raycastComponent._viewRange) + " viewRange " +  math.length((dir) * raycastComponent._viewRange) + " dir " + dir.x + " " + dir.y + " " + dir.z + " end " + end.x + " " + end.y + " " + end.z);
                RaycastInput raycastInput = new RaycastInput {
                    Start = pos,
                    End = end,
                    Filter = new CollisionFilter {
                        BelongsTo = ~((1u << 6) | (1u << 5)),
                        CollidesWith = ~((1u << 6) | (1u << 5)),
                        GroupIndex = 0
                    }
                };


                //var collector = new CustomRaycastCollector { RaycastingEntity = entity };
                var collector = new IgnoreEntityCollector(entity);
                
                Color color = Color.green;
                float rayLength = raycastComponent._viewRange;
                //s_MathPerfMarker.End();
                //s_RaycastShootPerfMarker.Begin();
                if (collisionWorld.CastRay(raycastInput, ref collector)) 
                //if (collisionWorld.CastRay(raycastInput, ref raycastHits) && raycastHits.Length > 1) 
                {
                    // Entity hitEntity = buildPhysicsWorldBodies[raycastHit.RigidBodyIndex].Entity;
                    // Debug.Log("printing raycastHits");
                    //NativeSortExtension.Sort<Unity.Physics.RaycastHit, RaycastHitDistanceComparer>(raycastHits, new RaycastHitDistanceComparer());
                    // foreach (Unity.Physics.RaycastHit hit in raycastHits)
                    // {
                    //     Debug.Log("entity " + entity.Index + " hit " + hit.Position + " " + hit.Fraction + " " + hit.SurfaceNormal + " " + hit.RigidBodyIndex + " " + hit.Entity + " " + hit.ColliderKey);
                    // }
                    // Debug.Log("end printing raycastHits");
                    // if (raycastHits.Length < 2)
                    // {
                    //     Debug.Log("raycastHits.Length < 2");
                    //     continue;
                    // }
                    //Entity hitEntity = buildPhysicsWorldBodies[raycastHits[1].RigidBodyIndex].Entity;
                    //Entity hitEntity = buildPhysicsWorldBodies[raycastHit.RigidBodyIndex].Entity;
                    //Entity hitEntity = buildPhysicsWorldBodies[collector.ClosestHit.RigidBodyIndex].Entity;
                    Entity hitEntity = collector.ClosestHit.Entity;
                    //rayLength = math.distance(raycastInput.Start, raycastHit.Fraction * raycastInput.End);
                    //rayLength = collector.MaxFraction;
                    rayLength = math.length(collector.ClosestHit.Position - pos);//math.mul(raycastInput.End, raycastHits[1].Fraction);
                    float whosThereTemp = 0f;
                    //if (hitEntity.collider.gameObject.name == "Prey")
                    //entityManager.GetComponent<PreyComponent>();
                    //ComponentDataFromEntity<PreyComponent> preyComponents = GetComponentDataFromEntity<PreyComponent>(true);
                    
                    //if (entityManager.HasComponent<PreyComponent>(hitEntity))
                    if (HasComponent<PreyComponent>(hitEntity))
                    {
                        whosThereTemp = 1.0f;
                    }
                    //else if (entityManager.HasComponent<PredatorComponent>(hitEntity))
                    else if (HasComponent<PredatorComponent>(hitEntity))
                    {
                        whosThereTemp = -1.0f;
                        color = Color.red;

                    }
                    //WhoIsThereDataElement initWhosThere = whosThere[i];
                    //initWhosThere.Value = whosThereTemp;
                    whosThere[i] = new WhoIsThereDataElement { Value = whosThereTemp };
                    //Debug.Log("whosthere " + i + " " + whosThere[i].Value);

                }
                else
                {
                    //print("I hit nothing");
                    color = Color.gray;
                    //WhoIsThereDataElement initWhosThere = whosThere[i];
                    //initWhosThere.Value = 0f;
                    whosThere[i] = new WhoIsThereDataElement { Value = 0f };
                    //Debug.Log("whosthere " + i + " " + whosThere[i].Value);

                }
                //Debug.Log("entity hit " + collector.FirstHitEntity + " rigid body " + collector.RigidBodyIndex);
                //Debug.Log("collector " + collector.ToString());
                //DistanceDataElement initDistance = distances[i];
                //initDistance.Value =  rayLength < raycastComponent._viewRange ? 1 / rayLength : 0;;
                distances[i] = new DistanceDataElement { Value = rayLength < raycastComponent._viewRange ? math.min(1 / rayLength, 1000f) : 0 };
                //Debug.Log("distance " + i + " " + distances[i].Value);
                //Distances[i] = rayLength < raycastComponent._viewRange ? 1 / rayLength : 0;
                //s_RaycastDrawRayMarker.Begin();
                if (raycastComponent.toggleShowRays){
                    //translation.Value = translation.Value + dir * rayLength;
                    //VectorLine.SetRay (Color.green, deltaTime, pos, math.normalize(dir) * rayLength);
                    Debug.DrawRay(pos, math.normalize(dir) * rayLength, color);
                }
                //s_RaycastDrawRayMarker.End();
                //s_RaycastShootPerfMarker.End();
                //Debug.Log("Raycast: " + math.normalize(dir) * rayLength + " pos " + pos + " color " + color);
                    
                //raycastHits.Dispose();
                
            }
                //RaycastHit2D hit = Physics2D.Raycast(pos, dir, raycastComponent._viewRange, _layerMask);

                
            //     if (hit.collider != null)
            //     {
            //         // print(
            //         //     "I can see a " + hit.collider.gameObject.name + " at " + hit.distance + " units"
            //         // );
                    
            // }
            //s_PreparePerfMarker.End();
        }).WithBurst().ScheduleParallel();//.ScheduleParallel();
        
        
    }

// private void InitNeurons(DynamicBuffer<NeuronDataElement> neurons, int[] layers)
// {
//     //List<float[]> NeuronsList = new List<float[]>();

//     for (int i = 0; i < layers.Length; i++)
//     {
//         neurons.Add(new float[layers[i]]);
//     }

//     neurons = NeuronsList.ToArray();
// }

/*
assigns a random value to each bias for each neuron between -0.5 and 0.5
*/
// private void InitBiases(DynamicBuffer<BiasDataElement> biases, int[] layers)
// {
//     //List<float[]> biasList = new List<float[]>();

//     for (int i = 0; i < layers.Length; i++)
//     {
//         //float[] bias = new float[layers[i]];

//         for (int j = 0; j < layers[i]; j++)
//         {
//             //bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
//             biases.Add(new BiasDataElement { Value = UnityEngine.Random.Range(-0.5f, 0.5f)});
//         }

//         //biasList.Add(bias);
//     }

//     //biases = biasList.ToArray();
// }

// private void InitWeights(DynamicBuffer<WeightDataElement> weights, int[] layers)
// {
//     //List<float[][]> WeightsList = new List<float[][]>();

//     for (int i = 0; i < layers.Length - 1; i++)
//     {
//         //List<float[]> layerWeightsList = new List<float[]>();
//         int neuronsInNextLayer = layers[i + 1];

//         for (int j = 0; j < layers[i]; j++)
//         {
//             //float[] neuronWeights = new float[neuronsInNextLayer];

//             for (int k = 0; k < neuronsInNextLayer; k++)
//             {
//                 //neuronWeights[k] = UnityEngine.Random.Range(-0.01f, 0.01f);
//                 weights.Add(new WeightDataElement { Value =  UnityEngine.Random.Range(-0.01f, 0.01f)});
//             }

//             //layerWeightsList.Add(neuronWeights);
//         }

//         //WeightsList.Add(layerWeightsList.ToArray());
//     }

//     //_weights = WeightsList.ToArray();
// }
}

// public struct CustomRaycastCollector : ICollector<Unity.Physics.RaycastHit>
// {
//     public Entity RaycastingEntity;
//     public Entity FirstHitEntity;

//     public bool EarlyOutOnFirstHit => false;
//     public float MaxFraction { get; set; }
//     public int NumHits { get; set; }
//     public int RigidBodyIndex { get; set; }

//     public bool AddHit(Unity.Physics.RaycastHit hit)
//     {
//         if (!hit.Entity.Equals(RaycastingEntity) && NumHits == 0)
//         {
//             FirstHitEntity = hit.Entity;
//             RigidBodyIndex = hit.RigidBodyIndex;
//             NumHits++;
//             MaxFraction = hit.Fraction;
//             return true; // Stop the raycast after the first hit
//         }
//         return false;
//     }
// }

  struct IgnoreEntityCollector : ICollector<Unity.Physics.RaycastHit>
    {
        // ICollector
        public bool EarlyOutOnFirstHit => false;
        public float MaxFraction { get; private set; }
        public int NumHits { get; private set; }
 
        // Input
        public Entity IgnoreEntity;
 
        // Output
        public Unity.Physics.RaycastHit ClosestHit { get; private set; }
 
        public IgnoreEntityCollector(Entity ignoreEntity)
        {
            MaxFraction = 1f;
            NumHits = 0;
            ClosestHit = default;
            IgnoreEntity = ignoreEntity;
        }
 
        public bool AddHit(Unity.Physics.RaycastHit hit)
        {
            if (hit.Entity == IgnoreEntity) return false;
            MaxFraction = hit.Fraction;
            ClosestHit = hit;
            NumHits = 1;
            return true;
        }
    }


public struct RaycastHitDistanceComparer : IComparer<Unity.Physics.RaycastHit>
{
    public int Compare(Unity.Physics.RaycastHit a, Unity.Physics.RaycastHit b)
    {
        return a.Fraction.CompareTo(b.Fraction);
    }
}



// using System.Collections;
// using System.Collections.Generic;
// using Unity.Entities;
// using Unity.Transforms;
// using UnityEngine;

// [AlwaysUpdateSystem]
// public partial class RaycastUpdateSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         //Debug.Log("num preys: " + SceneInitializerECS.NUMPREY);
//         Entities.ForEach((ref Translation translation, in LocalToWorld loc) =>
//         {
//             //raycast.origin = transform.position;
//             //raycast.direction = transform.forward;
//             if (float.IsNaN(translation.Value.x))
//             {
//                 Debug.Log("NAN");
//             }
//             //Debug.Log(translation.Value.x);
//         }).ScheduleParallel();
//     }
// }
