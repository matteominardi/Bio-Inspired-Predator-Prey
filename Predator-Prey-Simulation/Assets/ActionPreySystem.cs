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
public partial class ActionPreySystem : SystemBase
{
    public Unity.Mathematics.Random random;
    public int[] brainStructurePreys;
    public int dmgPreys;
    public float speedPreys;
    public int fovPreys;
    public int viewRangePreys;
    public int numRaysPreys;
    public int numNeuronsPreys;
    public int numWeightsPreys;
    public float mutationRate;
    public float mutationAmount;
    public Mesh meshAgent;
    public UnityEngine.Material materialPrey;

    private MovementInputActions _inputActions;
    private EntityCommandBufferSystem _ecb;
    private int _timerReproduction;
    private float _energyGain;

    protected override void OnStartRunning()
    {
        //random = new Unity.Mathematics.Random(56);
        // _inputActions = new MovementInputActions();
        // _inputActions.ManualMovement.MoveForward.performed += ctx => OnJump();
        // _inputActions.ManualMovement.MoveForward.canceled += ctx => OnJumpCanceled();
        // _inputActions.Enable();
        random.InitState((uint)math.abs((int)System.DateTime.Now.Ticks));
        brainStructurePreys = SceneInitializerECS._brainStructurePreys;
        dmgPreys = SceneInitializerECS.dmgPreys;
        speedPreys = SceneInitializerECS.speedPreys;
        fovPreys = SceneInitializerECS.fovPreys;
        viewRangePreys = SceneInitializerECS.viewRangePreys;
        numRaysPreys = SceneInitializerECS.numRaysPreys;
        mutationRate = SceneInitializerECS.mutationRate;
        mutationAmount = SceneInitializerECS.mutationAmount;
        numNeuronsPreys = 0;
        for (int i = 0; i < brainStructurePreys.Length; i++)
        {
            numNeuronsPreys += brainStructurePreys[i];
        }
        numWeightsPreys = 0;
        for (int i = 0; i < brainStructurePreys.Length - 1; i++)
        {
            numWeightsPreys += brainStructurePreys[i] * brainStructurePreys[i + 1];
        }
        meshAgent = SceneInitializerECS.MeshAgent;
        materialPrey = SceneInitializerECS.MaterialPrey;
        _ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _timerReproduction = SceneInitializerECS.timerReproductionPreys;
        _energyGain = SceneInitializerECS.energyGainPreys;

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


        float _timerReproduction = this._timerReproduction;
        float _energyGain = this._energyGain;
        Entities
        .WithAll<PreyTag>()
        .ForEach((Entity e, ref Translation translation, ref PhysicsVelocity velocity, ref PreyComponent preyComponent,  ref Rotation rotation, ref DynamicBuffer<OutputDataElement> outputs) => {
            if (!preyComponent.Alive)
            {
                return;
            }

            preyComponent.Fitness += 1.0f * deltaTime;

            if (preyComponent.Energy <= 0f)
            {
                //GetComponent<Raycast>().UpdateRays();
                preyComponent.Energy += _energyGain * deltaTime;
                preyComponent._energyExhausted = true;
                //renderMesh.material.SetColor("_Color", new Color(0, 0.3f, 0));
                //materialPrey.SetColor("_Color", Color.green);
                //GetComponent<SpriteRenderer>().color = new Color(0, 0.3f, 0);
                return;
            }

            if (preyComponent.Energy < 100f && preyComponent._energyExhausted)
            {
                preyComponent.Energy += _energyGain * deltaTime;
                if (preyComponent.Energy >= 100f)
                {
                    preyComponent.Energy = 100f;
                    preyComponent._energyExhausted = false;
                    //renderMesh.material.SetColor("_Color", Color.green);
                    //GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                }
                return;
            }
            
            if (arrowRight)
            {
                // move right
                //transform.Translate(Vector2.right * deltaTime * 2 );

                //rotate
                //rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-deltaTime * 90 * 2 * (int)preyComponent.Speed)));
                velocity.Angular.z = -deltaTime * 90 * math.min((int)preyComponent.Speed, 3);
                //transform.Rotate(new Vector3(0, 0, -1) * deltaTime * 90 * 2 * (int)preyComponent.Speed);
                //GetComponent<Raycast>().UpdateRays(-deltaTime * 90 * 2 * 0);
                //GetComponent<Raycast>().UpdateRays();
            }
            if (arrowLeft)
            {
                // move left
                //transform.Translate(-Vector2.right * deltaTime * 2 );

                //rotate
                //rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(deltaTime * 90 * 2 * (int)preyComponent.Speed)));
                velocity.Angular.z = deltaTime * 90 * math.min((int)preyComponent.Speed, 3);
                //transform.Rotate(new Vector3(0, 0, 1) * deltaTime * 90 * 2 * (int)preyComponent.Speed);
                //GetComponent<Raycast>().UpdateRays(deltaTime * 90 * 2);
                //GetComponent<Raycast>().UpdateRays();
            }
            float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
            if (arrowUp)
            {
                // move up
                //translation.Value += forward * deltaTime * 2 * (int)preyComponent.Speed;
                velocity.Linear = forward  * 2 * (int)preyComponent.Speed;
                //transform.Translate(Vector2.up * deltaTime * 2 * (int)preyComponent.Speed);
                //GetComponent<Raycast>().UpdateRays(0);
                //GetComponent<Raycast>().UpdateRays();
            }
            if (arrowDown)
            {
                // move down
                //float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
                //translation.Value += -forward * deltaTime * 2 * (int)preyComponent.Speed;
                velocity.Linear = -forward * 2 * (int)preyComponent.Speed;
                //transform.Translate(Vector2.down * deltaTime * 2 * (int)preyComponent.Speed);
                //GetComponent<Raycast>().UpdateRays(0);
                //GetComponent<Raycast>().UpdateRays();
            }

            // for (int i = 0; i < preyComponent._numRays; i++)
            // {
            //     _inputs[i * 2] = GetComponent<Raycast>().Distances[i];
            // }
            // for (int i = 0; i < _numRays; i++)
            // {
            //     _inputs[i * 2 + 1] = GetComponent<Raycast>().WhoIsThere[i];
            // }
            
            DynamicBuffer<float> _outputs = outputs.Reinterpret<float>(); //brain.FeedForward(_inputs);

            float angularVelocity = _outputs[0];
            float linearVelocity = _outputs[1];

            if ((angularVelocity == 0f && linearVelocity == 0f && !arrowUp && !arrowRight && !arrowLeft && !arrowDown))
            {
                if (preyComponent.Energy <= 100)
                    preyComponent.Energy = math.min(preyComponent.Energy + _energyGain * deltaTime, 100);
            }
            else
            {
                preyComponent.Energy -= _energyGain * deltaTime * math.max(math.abs(angularVelocity), math.abs(linearVelocity));
            }

            

            //float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
            //Debug.Log("linearVelocity: " + linearVelocity + " forward " + forward  + " deltatime " +  deltaTime + " speed: " + (int)preyComponent.Speed);
            //Debug.Log("linearVelocity update: " + forward * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity + " non-vector: " + deltaTime * 2 * (int)preyComponent.Speed * linearVelocity);
            //Debug.Log("angularVelocity: " + angularVelocity);
            translation.Value += forward * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity;
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(deltaTime * 90 * math.min((int)preyComponent.Speed, 3) * angularVelocity)));
            if (float.IsNaN(translation.Value.x))
            {
                //Debug.Log("translation is NaN");
                return;
            }
            float3 forwardMove = forward * 2 * (int)preyComponent.Speed * linearVelocity;
            if (float.IsNaN(forwardMove.x) || float.IsNaN(forwardMove.y) || float.IsNaN(forwardMove.z) || float.IsNaN(deltaTime * 90 * 2 * (int)preyComponent.Speed * angularVelocity))
            {
                //Debug.Log("forwardMove is NaN: " + forwardMove + " deltaTime: " + deltaTime + " angularVelocity: " + angularVelocity);
                return;
            }
            //velocity.Linear = forward * 2 * (int)preyComponent.Speed * linearVelocity;
            //velocity.Angular.z = deltaTime * 90 * 2 * (int)preyComponent.Speed * angularVelocity;

            //transform.Translate(Vector2.up * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity);
            //transform.Rotate(new Vector3(0, 0, angularVelocity) * deltaTime * 90 * 2 * (int)preyComponent.Speed);

            //GetComponent<Raycast>().UpdateRays();

        }).WithBurst().ScheduleParallel();//.WithBurst().ScheduleParallel();
        Dependency.Complete();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();
        Entities
        .WithAll<PreyTag>()
        .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref PreyComponent preyComponent, ref DynamicBuffer<LayerDataElement> layers, ref DynamicBuffer<NeuronDataElement> neurons, ref DynamicBuffer<BiasDataElement> biases, ref DynamicBuffer<WeightDataElement> weights) => {
            // if (preyComponent.Alive == false)
            // {
            //     parallelEcb.DestroyEntity(entityInQueryIndex, e);
            //     SceneInitializerECS.NUMPREY--;
            //     return;
            // }
            if (preyComponent.Alive && (int)preyComponent.Fitness % _timerReproduction == 0 && (math.floor(preyComponent.Fitness) / _timerReproduction) > preyComponent._counterReproduction && SceneInitializerECS.NUMPREY < SceneInitializerECS.MAXPREYALLOWED)
            {
                preyComponent._counterReproduction++;
                preyComponent._age = time;
                //Reproduce(translation, preyComponent.Generation, biases, weights, parallelEcb, entityInQueryIndex);

                // reproduce function
                if (SceneInitializerECS.NUMPREY == SceneInitializerECS.MAXPREYALLOWED) return;
                float2 posChild = random.NextFloat2(0, 1);
                float2 randomPosition = new float2(translation.Value.x, translation.Value.y) + posChild;
                //Vector2 randomPosition = new Vector2(translation.Value.x, translation.Value.y) + UnityEngine.Random.insideUnitCircle;
                float randomRotationAngle = random.NextFloat(0, 360); //UnityEngine.Random.Range(0.0f, 360.0f);
                quaternion randomRotation = quaternion.Euler(0.0f, 0.0f, math.radians(randomRotationAngle));
                //print("Random rotation " + randomRotationAngle);
                Entity child = parallelEcb.CreateEntity(entityInQueryIndex, SceneInitializerECS.PreyArchetype);
                parallelEcb.SetComponent(entityInQueryIndex, child, new Translation { Value = new float3(randomPosition.x, randomPosition.y, 0) });
                parallelEcb.SetComponent(entityInQueryIndex, child, new Rotation { Value = randomRotation });

                //child.GetComponent<Prey>().Start();
                //child.name = "Prey";

                parallelEcb.SetComponent<PreyComponent>(entityInQueryIndex, child, new PreyComponent {
                    Alive = true,
                    Lifepoints = 100,
                    Fitness = 0f,
                    Energy = 100,
                    Speed = speedPreys,
                    Generation = preyComponent.Generation+1,
                    _age = 0,
                    _counterReproduction = 0,
                    //_numRays = SceneInitializerECS.numRaysPreys,
                    _energyExhausted = false,
                    _dmg = dmgPreys,

                });

                parallelEcb.SetComponent(entityInQueryIndex, child, new RaycastComponent() { 
                        //physicsWorld = buildPhysicsWorld.PhysicsWorld,
                        _numberOfRays = numRaysPreys,
                        _fov = fovPreys,
                        _viewRange = viewRangePreys,
                        toggleShowRays = false
                });
                DynamicBuffer<DistanceDataElement> distancesChild = parallelEcb.AddBuffer<DistanceDataElement>(entityInQueryIndex, child);
                DynamicBuffer<WhoIsThereDataElement> whoIsThereChild = parallelEcb.AddBuffer<WhoIsThereDataElement>(entityInQueryIndex, child);
                DynamicBuffer<LayerDataElement> layersChild = parallelEcb.AddBuffer<LayerDataElement>(entityInQueryIndex, child);
                DynamicBuffer<NeuronDataElement> neuronsChild = parallelEcb.AddBuffer<NeuronDataElement>(entityInQueryIndex, child);
                DynamicBuffer<WeightDataElement> weightsChild = parallelEcb.AddBuffer<WeightDataElement>(entityInQueryIndex, child);
                DynamicBuffer<BiasDataElement> biasesChild = parallelEcb.AddBuffer<BiasDataElement>(entityInQueryIndex, child);
                DynamicBuffer<OutputDataElement> outputsChild = parallelEcb.AddBuffer<OutputDataElement>(entityInQueryIndex, child);
                distancesChild.ResizeUninitialized(numRaysPreys);
                whoIsThereChild.ResizeUninitialized(numRaysPreys);
                weightsChild.ResizeUninitialized(numWeightsPreys);
                biasesChild.ResizeUninitialized(numNeuronsPreys);
                layersChild.CopyFrom(layers);
                outputsChild.ResizeUninitialized(2);
                //int[] brainStructure = BrainStructure();
                //layersChild.Reinterpret<int>().CopyFrom(brainStructurePreys);
                //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
                
                neuronsChild.ResizeUninitialized(numNeuronsPreys);
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
                    material = materialPrey,
                    layerMask = 1
                });

                parallelEcb.SetComponent(entityInQueryIndex, child, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});

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
                SceneInitializerECS.NUMPREY++;
                
            }

            if (preyComponent.Alive && preyComponent._energyExhausted)
            {
                // ecb.SetSharedComponent(e, new RenderMesh {
                //     mesh = EntityManager.GetSharedComponentData<RenderMesh>(e).mesh,
                //     material = EntityManager.GetSharedComponentData<RenderMesh>(e).material
                // });//.material.SetColor("_Color", new Color(0, 0.3f, 0));
                parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 0.3f, 0, 1)});
            }
            else if (preyComponent.Alive)
            {
                //preyComponent.Energy += 5f * deltaTime;
                //if (preyComponent.Energy >= 100f)
                //{
                    
                    //EntityManager.GetSharedComponentData<RenderMesh>(e).material.SetColor("_Color", new Color(0, 1f, 0));
                    parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});
                //}
            }
            if (preyComponent.Alive == false)
            {
                parallelEcb.DestroyEntity(entityInQueryIndex, e);
                SceneInitializerECS.NUMPREY--;
            }
        }).WithoutBurst().Run();

        Dependency.Complete();
        //Debug.Log("Prey count: " + SceneInitializerECS.NUMPREY);
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
    //         Speed = speedPreys,
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
