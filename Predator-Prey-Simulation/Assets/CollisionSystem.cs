using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateAfter(typeof(StepPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
// [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
[UpdateAfter(typeof(ExportPhysicsWorld))]

//[UpdateBefore(typeof(ActionPreySystem))]
//[UpdateBefore(typeof(ActionPredatorSystem))]
public partial class CollisionSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;
    private float _reproductionGainPredators;

    protected override void OnStartRunning()
    {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        this.RegisterPhysicsRuntimeSystemReadOnly();
        _reproductionGainPredators = SceneInitializerECS.reprodGainWhenEatPredators;
    }

    protected override void OnUpdate()
    {
        var job = new CollisionSystemJob();
        job.predatorGroup = GetComponentDataFromEntity<PredatorTag>(true);
        job.preyGroup = GetComponentDataFromEntity<PreyTag>(true);
        job.predatorComponentGroup = GetComponentDataFromEntity<PredatorComponent>(false);
        job.preyComponentGroup = GetComponentDataFromEntity<PreyComponent>(false);
        //job.commandBuffer = commandBufferSystem.CreateCommandBuffer();
        //job.numPrey = SceneInitializerECS.NUMPREY;
        //job.numPredator = SceneInitializerECS.NUMPREDATOR;
        job.reproductionGainPredators = _reproductionGainPredators;
        //NativeArray<int> results = new NativeArray<int>(2, Allocator.TempJob);
        //results[0] = SceneInitializerECS.NUMPREY;
        //results[1] = SceneInitializerECS.NUMPREDATOR;
        //job.results = results;

        var dependencies = JobHandle.CombineDependencies(this.Dependency, _stepPhysicsWorld.FinalSimulationJobHandle);
        Dependency = job.Schedule(_stepPhysicsWorld.Simulation, dependencies);
        //commandBufferSystem.AddJobHandleForProducer(Dependency);
        //Dependency.Complete();
        //SceneInitializerECS.NUMPREY = results[0];
        //SceneInitializerECS.NUMPREDATOR = results[1];
        //Debug.Log("num preys: " + SceneInitializerECS.NUMPREY + " num predators: " + SceneInitializerECS.NUMPREDATOR);
        //results.Dispose();
    }

    [BurstCompile]
    struct CollisionSystemJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PreyTag> preyGroup;
        [ReadOnly] public ComponentDataFromEntity<PredatorTag> predatorGroup;
        [ReadOnly] public float reproductionGainPredators;
        public ComponentDataFromEntity<PreyComponent> preyComponentGroup;
        public ComponentDataFromEntity<PredatorComponent> predatorComponentGroup;
        public EntityCommandBuffer commandBuffer;
        public int numPrey;
        public int numPredator;
        //public NativeArray<int> results;
        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isPreyA = preyGroup.HasComponent(entityA);
            bool isPreyB = preyGroup.HasComponent(entityB);
            bool isPredatorA = predatorGroup.HasComponent(entityA);
            bool isPredatorB = predatorGroup.HasComponent(entityB);
            //Debug.Log("isprey " + isPreyA + " " + isPreyB + " ispredator " + isPredatorA + " " + isPredatorB);
            //results[0] = numPrey;
            //results[1] = numPredator;

            if ((isPreyA && isPreyB) || (isPredatorA && isPredatorB))
            {
                return;
            }
            //Debug.Log("after isprey " + isPreyA + " " + isPreyB + " ispredator " + isPredatorA + " " + isPredatorB);

            Entity preyEntity = isPreyA ? entityA : entityB;
            Entity predatorEntity = isPredatorA ? entityA : entityB;

            PreyComponent preyComponent = preyComponentGroup[preyEntity];
            PredatorComponent predatorComponent = predatorComponentGroup[predatorEntity];

            float _timerStopCollisionPrey = preyComponent._timerStopCollision;
            float _timerStopCollisionPredator = predatorComponent._timerStopCollision;

            if (_timerStopCollisionPrey > 0 && _timerStopCollisionPredator > 0)
            {
                return;
            }
            preyComponent._timerStopCollision = 0.5f;
            predatorComponent._timerStopCollision = 0.5f;

            preyComponent.Lifepoints -= predatorComponent._dmg;
            predatorComponent.Lifepoints -= preyComponent._dmg;



            //Debug.Log("predator lifepoints: " + predatorComponent.Lifepoints + " prey lifepoints: " + preyComponent.Lifepoints + " predator dmg: " + predatorComponent._dmg + " prey dmg: " + preyComponent._dmg);
            if (preyComponent.Lifepoints <= 0)
            {
                preyComponent.Alive = false;
                preyComponent.Lifepoints = 0;
                predatorComponent.Fitness += 5;
                predatorComponent.Energy = math.min(predatorComponent.Energy + 10, 100);
                predatorComponent.Lifepoints = math.min(predatorComponent.Lifepoints + (int)(reproductionGainPredators*1.5f), 100);
                predatorComponent.ReproductionFactor += reproductionGainPredators;
                
                //commandBuffer.DestroyEntity(preyEntity);
                //results[0] = numPrey - 1;//SceneInitializerECS.NUMPREY--;
                //Debug.Log("scene init num prey: " + SceneInitializerECS.NUMPREY + " num predator: " + SceneInitializerECS.NUMPREDATOR);

            }


            if (predatorComponent.Lifepoints <= 0)
            {
                predatorComponent.Alive = false;
                predatorComponent.Fitness = -1;
                predatorComponent.Lifepoints = 0;
                //predatorComponentGroup[predatorEntity] = predatorComponent;
                //commandBuffer.DestroyEntity(predatorEntity);
                //SceneInitializerECS.NUMPREDATOR--;
                //results[1] = numPredator - 1;
            }
            preyComponentGroup[preyEntity] = preyComponent;
            predatorComponentGroup[predatorEntity] = predatorComponent;
            // preyComponentGroup[preyEntity] = preyComponent;
            // predatorComponentGroup[predatorEntity] = predatorComponent;
            //predatorComponent.Energy += predatorComponent._dmg;


        }
    }
}

// [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateBefore(typeof(EndFramePhysicsSystem))]
// [UpdateAfter(typeof(StepPhysicsWorld))]
// //[UpdateAfter(typeof(EndFramePhysicsSystem))]
// public partial class CollisionSystem : SystemBase
// {
//     private BuildPhysicsWorld _buildPhysicsWorld;
//     private StepPhysicsWorld _stepPhysicsWorld;
//     private CollisionWorld _collisionWorld;
//     private AABB _aabb;

//     protected override void OnCreate()
//     {
//         base.OnCreate();
//         _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//         _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        
//         _aabb = new AABB { Center = float3.zero, Extents = new float3(1, 1, 1) }; // Adjust the AABB values as needed
//     }
//     protected override void OnStartRunning()
//     {
        
//     }
//     protected override void OnUpdate()
//     {   
//         var aabb = _aabb;
//         _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
//         var healthReductionAmount = 1f * Time.DeltaTime; // Adjust the health reduction amount as needed
//         var hitList = new Unity.Collections.NativeList<int>(1024, Allocator.Temp);
//         _collisionWorld.OverlapAabb(
//             new OverlapAabbInput
//             {
//                 Aabb = new Aabb { Max = myMaxValue, Min = myMinValue },
//                 Filter = new CollisionFilter
//                 {
//                     BelongsTo = ~0u,
//                     CollidesWith = ~0u,
//                     GroupIndex = 0,
//                 }
//             },
//             ref hitList
//         );

//         Entities
//             .ForEach((ref PreyComponent preyComponent, in Translation translation) =>
//             {
//                 aabb.Center = translation.Value;

//                 var aabbOverlapCollector = new AabbOverlapCollector(aabb);
//                 physicsWorld.OverlapAabb(aabbOverlapCollector, out OverlapAabbOutputs overlapResults);

//                 if (overlapResults.NumHits > 0)
//                 {
//                     health.Value -= healthReductionAmount;
//                 }
//             }).ScheduleParallel();

//         _buildPhysicsWorld.AddInputDependency(Dependency);
//         _stepPhysicsWorld.EnqueueFinalJobHandle(Dependency);
//     }

//     private struct AabbOverlapCollector : ICollector<Physics.RaycastHit>
//     {
//         public bool EarlyOutOnHit => false;
//         public int MaxHits => 1;
//         public int NumHits { get; private set; }

//         private readonly AABB _aabb;

//         public AabbOverlapCollector(AABB aabb)
//         {
//             _aabb = aabb;
//             NumHits = 0;
//         }

//         public bool AddHit(Physics.RaycastHit hit)
//         {
//             if (_aabb.Contains(hit.Position))
//             {
//                 NumHits++;
//                 return true;
//             }
//             return false;
//         }
//     }
    
// }
