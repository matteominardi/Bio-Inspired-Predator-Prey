using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(NNSystem))]
public partial class ActionPredatorSystem : SystemBase
{
    public Unity.Mathematics.Random random;
    public int[] brainStructurePredators;
    public int dmgPredators;
    public float speedPredators;
    public int fovPredators;
    public int viewRangePredators;
    public int numRaysPredators;
    public int numNeuronsPredators;
    public int numWeightsPredators;
    public float mutationRate;
    public float mutationAmount;
    public Mesh meshAgent;
    public UnityEngine.Material materialPredator;
    private float _energyLoss;

    //private MovementInputActions _inputActions;
    //private EntityCommandBufferSystem _ecb;

    protected override void OnStartRunning()
    {
        //random = new Unity.Mathematics.Random(56);
        // _inputActions = new MovementInputActions();
        // _inputActions.ManualMovement.MoveForward.performed += ctx => OnJump();
        // _inputActions.ManualMovement.MoveForward.canceled += ctx => OnJumpCanceled();
        // _inputActions.Enable();
        random.InitState((uint)math.abs((int)System.DateTime.Now.Ticks));
        brainStructurePredators = SceneInitializerECS._brainStructurePredators;
        dmgPredators = SceneInitializerECS.dmgPredators;
        speedPredators = SceneInitializerECS.speedPredators;
        fovPredators = SceneInitializerECS.fovPredators;
        viewRangePredators = SceneInitializerECS.viewRangePredators;
        numRaysPredators = SceneInitializerECS.numRaysPredators;
        mutationRate = SceneInitializerECS.mutationRate;
        mutationAmount = SceneInitializerECS.mutationAmount;
        numNeuronsPredators = 0;
        for (int i = 0; i < brainStructurePredators.Length; i++)
        {
            numNeuronsPredators += brainStructurePredators[i];
        }
        numWeightsPredators = 0;
        for (int i = 0; i < brainStructurePredators.Length - 1; i++)
        {
            numWeightsPredators += brainStructurePredators[i] * brainStructurePredators[i + 1];
        }
        meshAgent = SceneInitializerECS.MeshAgent;
        materialPredator = SceneInitializerECS.MaterialPredator;
        _energyLoss = SceneInitializerECS.energyLossPredators;
        //_reproductionGain = SceneInitializerECS.reprodGainWhenEatPredators;
        //_ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        bool arrowDown = Input.GetKey(KeyCode.DownArrow);
        bool arrowUp = Input.GetKey(KeyCode.UpArrow);
        bool arrowLeft = Input.GetKey(KeyCode.LeftArrow);
        bool arrowRight = Input.GetKey(KeyCode.RightArrow);
        //Debug.Log("arrowDown: " + arrowDown + " arrowUp: " + arrowUp + " arrowLeft: " + arrowLeft + " arrowRight: " + arrowRight);
        

        //var parallelEcb = _ecb.CreateCommandBuffer().AsParallelWriter(); //new EntityCommandBuffer(Allocator.TempJob);

        float energyLoss = this._energyLoss;
        Entities
        .WithAll<PredatorTag>()
        .ForEach((Entity e, ref Translation translation, ref PhysicsVelocity velocity, ref PredatorComponent predatorComponent,  ref Rotation rotation, ref DynamicBuffer<OutputDataElement> outputs) => {
            
            if (!predatorComponent.Alive)
                return;

            // if (ReproductionFactor >= 100 && CanReproduce)
            // if (ReproductionFactor >= 100 && CanReproduce.CanPredatorsReproduce())

            // if (predatorComponent.ReproductionFactor >= 100 && Counter < MaxPredator)
            // {
            //     //print("I am reproducing");
            //     Reproduce(brain, Generation);
            //     ReproductionFactor = 0;
            // }

            predatorComponent.Fitness += 1.0f * deltaTime;
            predatorComponent.ReproductionFactor = math.max(predatorComponent.ReproductionFactor - energyLoss * deltaTime, 0);
            //GetComponent<Raycast>().UpdateRays(0);

            // if (predatorComponent.Energy <= 0)
            // {
            //     //print("I am dead");
            //     predatorComponent.Alive = false;
            //     predatorComponent.Fitness = -1;
            //     Counter--;
            //     Destroy(gameObject);
            // }

            // _-----------------------____-----___--_-__-----__-
            
            
            if (arrowRight)
            {
                //rotate
                velocity.Angular.z = -deltaTime * 90 * math.min((int)predatorComponent.Speed, 3);
            }
            if (arrowLeft)
            {
                //rotate
                velocity.Angular.z = deltaTime * 90 * math.min((int)predatorComponent.Speed, 3);
            }
            float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
            if (arrowUp)
            {
                // move up
                velocity.Linear = forward  * 2 * (int)predatorComponent.Speed;
            }
            if (arrowDown)
            {
                // move down
                velocity.Linear = -forward * 2 * (int)predatorComponent.Speed;
            }

            
            DynamicBuffer<float> _outputs = outputs.Reinterpret<float>(); //brain.FeedForward(_inputs);

            float angularVelocity = _outputs[0];
            float linearVelocity = _outputs[1];

            if ((angularVelocity == 0f && linearVelocity == 0f && !arrowUp && !arrowRight && !arrowLeft && !arrowDown))
            {
                //if (predatorComponent.Energy <= 100)
                //    predatorComponent.Energy = math.min(predatorComponent.Energy + 5f * deltaTime, 100);
            }
            else
            {
                predatorComponent.Energy -= energyLoss * deltaTime * math.max(math.max(math.abs(angularVelocity), math.abs(linearVelocity)), 0.3f);
            }

            

            //float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
            //Debug.Log("linearVelocity: " + linearVelocity + " forward " + forward  + " deltatime " +  deltaTime + " speed: " + (int)predatorComponent.Speed);
            //Debug.Log("linearVelocity update: " + forward * deltaTime * 2 * (int)predatorComponent.Speed * linearVelocity + " non-vector: " + deltaTime * 2 * (int)predatorComponent.Speed * linearVelocity);
            //Debug.Log("angularVelocity: " + angularVelocity);
            translation.Value += forward * deltaTime * 2 * (int)predatorComponent.Speed * linearVelocity;
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(deltaTime * 90 * math.min((int)predatorComponent.Speed, 3) * angularVelocity)));
            if (float.IsNaN(translation.Value.x))
            {
                //Debug.Log("translation is NaN");
                return;
            }
            float3 forwardMove = forward * 2 * (int)predatorComponent.Speed * linearVelocity;
            if (float.IsNaN(forwardMove.x) || float.IsNaN(forwardMove.y) || float.IsNaN(forwardMove.z) || float.IsNaN(deltaTime * 90 * 2 * (int)predatorComponent.Speed * angularVelocity))
            {
                //Debug.Log("forwardMove is NaN: " + forwardMove + " deltaTime: " + deltaTime + " angularVelocity: " + angularVelocity);
                return;
            }
            //velocity.Linear = forward * 2 * (int)predatorComponent.Speed * linearVelocity;
            //velocity.Angular.z = deltaTime * 90 * 2 * (int)predatorComponent.Speed * angularVelocity;

            //transform.Translate(Vector2.up * deltaTime * 2 * (int)predatorComponent.Speed * linearVelocity);
            //transform.Rotate(new Vector3(0, 0, angularVelocity) * deltaTime * 90 * 2 * (int)predatorComponent.Speed);

            //GetComponent<Raycast>().UpdateRays();

        }).WithBurst().ScheduleParallel();//.WithBurst().ScheduleParallel();
        Dependency.Complete();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();
        Entities
        .WithAll<PredatorTag>()
        .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref PredatorComponent predatorComponent, ref DynamicBuffer<LayerDataElement> layers, ref DynamicBuffer<NeuronDataElement> neurons, ref DynamicBuffer<BiasDataElement> biases, ref DynamicBuffer<WeightDataElement> weights) => {
            //if ((int)predatorComponent.Fitness % 5 == 0 && (math.floor(predatorComponent.Fitness) / 5) > predatorComponent._counterReproduction && SceneInitializerECS.NUMpredator < SceneInitializerECS.MAXpredatorALLOWED)
            // if (predatorComponent.Alive == false || predatorComponent.Energy <= 0)
            // {
            //     parallelEcb.DestroyEntity(entityInQueryIndex, e);
            //     SceneInitializerECS.NUMPREDATOR--;
            //     return;
            // }
            if (predatorComponent.Alive && predatorComponent.ReproductionFactor >= 100 && SceneInitializerECS.NUMPREDATOR < SceneInitializerECS.MAXPREDATORALLOWED)
            {
                //predatorComponent._counterReproduction++;
                //predatorComponent._age = time;
                //Reproduce(translation, predatorComponent.Generation, biases, weights, parallelEcb, entityInQueryIndex);

                // reproduce function
                float2 posChild = random.NextFloat2(0, 1);
                float2 randomPosition = new float2(translation.Value.x, translation.Value.y) + posChild;
                //Vector2 randomPosition = new Vector2(translation.Value.x, translation.Value.y) + UnityEngine.Random.insideUnitCircle;
                float randomRotationAngle = random.NextFloat(0, 360); //UnityEngine.Random.Range(0.0f, 360.0f);
                quaternion randomRotation = quaternion.Euler(0.0f, 0.0f, math.radians(randomRotationAngle));
                //print("Random rotation " + randomRotationAngle);
                Entity child = parallelEcb.CreateEntity(entityInQueryIndex, SceneInitializerECS.PredatorArchetype);
                parallelEcb.SetComponent(entityInQueryIndex, child, new Translation { Value = new float3(randomPosition.x, randomPosition.y, 0) });
                parallelEcb.SetComponent(entityInQueryIndex, child, new Rotation { Value = randomRotation });

                //child.GetComponent<predator>().Start();
                //child.name = "predator";

                parallelEcb.SetComponent<PredatorComponent>(entityInQueryIndex, child, new PredatorComponent {
                    Alive = true,
                    Lifepoints = 100,
                    Fitness = 0f,
                    Energy = 100,
                    Speed = speedPredators,
                    Generation = predatorComponent.Generation+1,
                    //_age = 0,
                    ReproductionFactor = 0,
                    //_numRays = SceneInitializerECS.numRaysPredators,
                    //_energyExhausted = false,
                    _dmg = dmgPredators,

                });

                parallelEcb.SetComponent(entityInQueryIndex, child, new RaycastComponent() { 
                        //physicsWorld = buildPhysicsWorld.PhysicsWorld,
                        _numberOfRays = numRaysPredators,
                        _fov = fovPredators,
                        _viewRange = viewRangePredators,
                        toggleShowRays = false
                });
                DynamicBuffer<DistanceDataElement> distancesChild = parallelEcb.AddBuffer<DistanceDataElement>(entityInQueryIndex, child);
                DynamicBuffer<WhoIsThereDataElement> whoIsThereChild = parallelEcb.AddBuffer<WhoIsThereDataElement>(entityInQueryIndex, child);
                DynamicBuffer<LayerDataElement> layersChild = parallelEcb.AddBuffer<LayerDataElement>(entityInQueryIndex, child);
                DynamicBuffer<NeuronDataElement> neuronsChild = parallelEcb.AddBuffer<NeuronDataElement>(entityInQueryIndex, child);
                DynamicBuffer<WeightDataElement> weightsChild = parallelEcb.AddBuffer<WeightDataElement>(entityInQueryIndex, child);
                DynamicBuffer<BiasDataElement> biasesChild = parallelEcb.AddBuffer<BiasDataElement>(entityInQueryIndex, child);
                DynamicBuffer<OutputDataElement> outputsChild = parallelEcb.AddBuffer<OutputDataElement>(entityInQueryIndex, child);
                distancesChild.ResizeUninitialized(numRaysPredators);
                whoIsThereChild.ResizeUninitialized(numRaysPredators);
                weightsChild.ResizeUninitialized(numWeightsPredators);
                biasesChild.ResizeUninitialized(numNeuronsPredators);
                layersChild.CopyFrom(layers);
                outputsChild.ResizeUninitialized(2);
                //int[] brainStructure = BrainStructure();
                //layersChild.Reinterpret<int>().CopyFrom(brainStructurePredators);
                //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
                
                neuronsChild.ResizeUninitialized(numNeuronsPredators);
                //DynamicBuffer<BiasDataElement> biasesChild = EntityManager.GetBuffer<BiasDataElement>(child);
                biases.CopyFrom(biases);
                //DynamicBuffer<WeightDataElement> weightsChild = EntityManager.GetBuffer<WeightDataElement>(child);
                weights.CopyFrom(weights);

                // mutate function
                float mutationRate = SceneInitializerECS.mutationRate;
                float mutationAmount = SceneInitializerECS.mutationAmount;
                for (int i = 0; i < biasesChild.Length; i++)
                {
                    //for (int j = 0; j < biases[i].Length; j++)
                    //{
                        biasesChild[i] = (random.NextFloat(0f, 1f) <= mutationRate) ? biases[i] = new BiasDataElement { Value = biases[i].Value + random.NextFloat(-mutationAmount, mutationAmount)}  : biases[i];
                    //}
                }

                for (int i = 0; i < weightsChild.Length; i++)
                {
                    //for (int j = 0; j < weights[i].Length; j++)
                    //{
                    //    for (int k = 0; k < _weights[i][j].Length; k++)
                    //    {
                            weightsChild[i] = (random.NextFloat(0f, 1f) <= mutationRate) ? weights[i] = new WeightDataElement { Value = weights[i].Value + random.NextFloat(-mutationAmount, mutationAmount)} : weights[i];

                    //    }
                    //}
                }

                // end mutate function

                parallelEcb.SetSharedComponent(entityInQueryIndex, child, new RenderMesh() { 
                    mesh = meshAgent,
                    material = materialPredator,
                    layerMask = 1
                });

                parallelEcb.SetComponent(entityInQueryIndex, child, new URPMaterialPropertyBaseColor {Value = new float4(1f, 0f, 0, 1)});

                parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsMass {
                    InverseMass = 1f,
                    InverseInertia = new float3(1f, 1f, 1f),
                    Transform = RigidTransform.identity
                });

                var material = new Unity.Physics.Material
                {
                    //CustomTags = Unity.Physics.Material.Default.CustomTags,
                    CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents,
                    //Friction = Unity.Physics.Material.Default.Friction,
                    //FrictionCombinePolicy = Unity.Physics.Material.Default.FrictionCombinePolicy,
                    //Restitution = Unity.Physics.Material.Default.Restitution,
                    //RestitutionCombinePolicy = Unity.Physics.Material.Default.RestitutionCombinePolicy,
                };
                var collisionFilter = new Unity.Physics.CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                };


                parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
                    Center = new float3(0f, 0f, 0f),
                    Radius = 0.5f,

                }, CollisionFilter.Default, material) });

                // parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsCollider { Value = Unity.Physics.CylinderCollider.Create(new CylinderGeometry() { 
                //     Center = new float3(0f, 0f, 0f),
                //     Radius = 0.5f,
                //     Height = 1f,
                //     Orientation = quaternion.Euler(0f, 0f, math.radians(0f)),
                //     SideCount = 3
                // }, Unity.Physics.CollisionFilter.Default) });

                parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsDamping()
                {
                    Linear = 4f,
                    Angular = 4f
                });

                parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsGravityFactor()
                {
                    Value = 0f
                });

                predatorComponent.ReproductionFactor = 0;
                SceneInitializerECS.NUMPREDATOR++;
                
                if (predatorComponent.Energy <= 0)
                {
                    //print("I am dead");
                    predatorComponent.Alive = false;
                    predatorComponent.Fitness = -1;
                    //Counter--;
                    SceneInitializerECS.NUMPREDATOR--;
                    parallelEcb.DestroyEntity(entityInQueryIndex, e);
                }
            }
            if (predatorComponent.Alive == false || predatorComponent.Energy <= 0)
            {
                parallelEcb.DestroyEntity(entityInQueryIndex, e);
                SceneInitializerECS.NUMPREDATOR--;
            }

            // if (predatorComponent._energyExhausted)
            // {
            //     // ecb.SetSharedComponent(e, new RenderMesh {
            //     //     mesh = EntityManager.GetSharedComponentData<RenderMesh>(e).mesh,
            //     //     material = EntityManager.GetSharedComponentData<RenderMesh>(e).material
            //     // });//.material.SetColor("_Color", new Color(0, 0.3f, 0));
            //     parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 0.3f, 0, 1)});
            // }
            // else
            // {
            //     //predatorComponent.Energy += 5f * deltaTime;
            //     //if (predatorComponent.Energy >= 100f)
            //     //{
                    
            //         //EntityManager.GetSharedComponentData<RenderMesh>(e).material.SetColor("_Color", new Color(0, 1f, 0));
            //         parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});
            //     //}
            // }
        }).WithoutBurst().Run();

        Dependency.Complete();
        //Debug.Log("predator count: " + SceneInitializerECS.NUMpredator);
        ecb.Playback(this.EntityManager);
        ecb.Dispose();
    }

    // public void Reproduce(Translation translation, int generation, DynamicBuffer<BiasDataElement> biasesParent, DynamicBuffer<WeightDataElement> weightsParent, EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex) 
    // {
    //     if (SceneInitializerECS.NUMPREY == SceneInitializerECS.MAXPREYALLOWED) return;
    //     Vector2 randomPosition = new Vector2(translation.Value.x, translation.Value.y) + UnityEngine.Random.insideUnitCircle;
    //     float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
    //     quaternion randomRotation = quaternion.Euler(0.0f, 0.0f, math.radians(randomRotationAngle));
    //     //print("Random rotation " + randomRotationAngle);
    //     Entity child = ecb.CreateEntity(entityInQueryIndex, SceneInitializerECS.PreyArchetype);
    //     ecb.SetComponent(entityInQueryIndex, child, new Translation { Value = new float3(randomPosition.x, randomPosition.y, 0) });
    //     ecb.SetComponent(entityInQueryIndex, child, new Rotation { Value = randomRotation });

    //     //child.GetComponent<Prey>().Start();
    //     //child.name = "Prey";

    //     ecb.SetComponent<PreyComponent>(entityInQueryIndex, child, new PreyComponent {
    //         Alive = true,
    //         Lifepoints = 100,
    //         Fitness = 0f,
    //         Energy = 100,
    //         Speed = speedPredators,
    //         Generation = generation+1,
    //         _age = 0,
    //         _counterReproduction = 0,
    //         //_numRays = SceneInitializerECS.numRaysPreys,
    //         _energyExhausted = false,
    //         _dmg = dmgPreys,

    //     });

    //     ecb.SetComponent(entityInQueryIndex, child, new RaycastComponent() { 
    //             //physicsWorld = buildPhysicsWorld.PhysicsWorld,
    //             _numberOfRays = numRaysPreys,
    //             _fov = fovPreys,
    //             _viewRange = viewRangePreys,
    //             toggleShowRays = false
    //     });
    //     ecb.AddBuffer<DistanceDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<WhoIsThereDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<LayerDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<NeuronDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<WeightDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<BiasDataElement>(entityInQueryIndex, child);
    //     ecb.AddBuffer<OutputDataElement>(entityInQueryIndex, child);
    //     EntityManager.GetBuffer<DistanceDataElement>(child).ResizeUninitialized(numRaysPreys);
    //     EntityManager.GetBuffer<WhoIsThereDataElement>(child).ResizeUninitialized(numRaysPreys);
    //     //int[] brainStructure = BrainStructure();
    //     EntityManager.GetBuffer<LayerDataElement>(child).Reinterpret<int>().CopyFrom(brainStructurePreys);
    //     //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
        
    //     EntityManager.GetBuffer<NeuronDataElement>(child).ResizeUninitialized(numNeuronsPreys);
    //     DynamicBuffer<BiasDataElement> biases = EntityManager.GetBuffer<BiasDataElement>(child);
    //     biases.CopyFrom(biasesParent);
    //     DynamicBuffer<WeightDataElement> weights = EntityManager.GetBuffer<WeightDataElement>(child);
    //     weights.CopyFrom(weightsParent);

    //     Mutate(biases.Reinterpret<float>(), weights.Reinterpret<float>(), numNeuronsPreys, numWeightsPreys, mutationRate, mutationAmount);

    //     ecb.SetSharedComponent(entityInQueryIndex, child, new RenderMesh() { 
    //         mesh = meshAgent,
    //         material = materialPrey,
    //         layerMask = 1
    //     });

    //     ecb.SetComponent(entityInQueryIndex, child, new PhysicsMass {
    //         InverseMass = 1f,
    //         InverseInertia = new float3(1f, 1f, 1f),
    //         Transform = RigidTransform.identity
    //     });

    //     ecb.SetComponent(entityInQueryIndex, child, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
    //         Center = new float3(0f, 0f, 0f),
    //         Radius = 0.5f
    //     }, Unity.Physics.CollisionFilter.Default) });

    //     ecb.SetComponent(entityInQueryIndex, child, new PhysicsDamping()
    //     {
    //         Linear = 4f,
    //         Angular = 4f
    //     });

    //     ecb.SetComponent(entityInQueryIndex, child, new PhysicsGravityFactor()
    //     {
    //         Value = 0f
    //     });

    //     //child.GetComponent<Prey>().Generate(generation + 1, Speed, _dmg, _fov, _numRays, _viewRange, parent.Copy(new NeuralNetwork(BrainModel)));//, randomRotationAngle);
    //     //Counter++;
    // }

    // public void Mutate(DynamicBuffer<float> biases, DynamicBuffer<float> weights, int numBiases, int numWeights, float mutationRate, float mutationAmount)
    // {
    //     for (int i = 0; i < biases.Length; i++)
    //     {
    //         //for (int j = 0; j < biases[i].Length; j++)
    //         //{
    //             biases[i] = (UnityEngine.Random.Range(0f, 1f) <= mutationRate) ? biases[i] += UnityEngine.Random.Range(-mutationAmount, mutationAmount) : biases[i];
    //         //}
    //     }

    //     for (int i = 0; i < weights.Length; i++)
    //     {
    //         //for (int j = 0; j < weights[i].Length; j++)
    //         //{
    //         //    for (int k = 0; k < _weights[i][j].Length; k++)
    //         //    {
    //                 weights[i] = (UnityEngine.Random.Range(0f, 1f) <= mutationRate) ? weights[i] += UnityEngine.Random.Range(-mutationAmount, mutationAmount) : weights[i];

    //         //    }
    //         //}
    //     }
    // }
}
