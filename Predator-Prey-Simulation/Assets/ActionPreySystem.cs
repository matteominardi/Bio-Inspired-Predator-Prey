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
//[UpdateAfter(typeof(CollisionSystem))]
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

    //private MovementInputActions _inputActions;
    //private EntityCommandBufferSystem _ecb;
    private int _timerReproduction;
    private float _energyGain;
    
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    private float _angularVelocityFactor;
    private float _linearVelocityFactor;

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
        //_ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _timerReproduction = SceneInitializerECS.timerReproductionPreys;
        _energyGain = SceneInitializerECS.energyGainPreys;

        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        _angularVelocityFactor = 0.008f * 90 * math.min(speedPreys, 6f);
        _linearVelocityFactor = speedPreys;

    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        bool arrowDown = Input.GetKey(KeyCode.DownArrow);
        bool arrowUp = Input.GetKey(KeyCode.UpArrow);
        bool arrowLeft = Input.GetKey(KeyCode.LeftArrow);
        bool arrowRight = Input.GetKey(KeyCode.RightArrow);
        bool mKey = Input.GetKeyDown(KeyCode.M);
        if (mKey)
        {
            SceneInitializerECS.ManualMovement = !SceneInitializerECS.ManualMovement;
        }
        //Debug.Log("arrowDown: " + arrowDown + " arrowUp: " + arrowUp + " arrowLeft: " + arrowLeft + " arrowRight: " + arrowRight);
        

        //var parallelEcb = _ecb.CreateCommandBuffer().AsParallelWriter(); //new EntityCommandBuffer(Allocator.TempJob);
        float _timerReproduction = this._timerReproduction;
        float _energyGain = this._energyGain;
        bool _manualMovement = SceneInitializerECS.ManualMovement;
        float _angularVelocityFactor = this._angularVelocityFactor;
        float _linearVelocityFactor = this._linearVelocityFactor;

        
        //EntityCommandBuffer.ParallelWriter parallelEcb = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
        .WithAll<PreyTag>()
        .ForEach((Entity e, ref Translation translation, ref PhysicsVelocity velocity, ref PreyComponent preyComponent,  ref Rotation rotation, ref DynamicBuffer<OutputDataElement> outputs) => {
            if (!preyComponent.Alive)
            {
                //parallelEcb.DestroyEntity(entityInQueryIndex, e);
                return;
            }

            preyComponent.Fitness += 1.0f * deltaTime;

            if (preyComponent._timerStopCollision > 0f)
            {
                preyComponent._timerStopCollision -= deltaTime;
            }

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

            preyComponent._age += 1.0f * deltaTime;

            float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));

            if (_manualMovement)
            {
                if (arrowRight)
                {
                    // move right
                    //transform.Translate(Vector2.right * deltaTime * 2 );

                    //rotate
                    //rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-deltaTime * 90 * 2 * (int)preyComponent.Speed)));
                    velocity.Angular.z = -_angularVelocityFactor;
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
                    velocity.Angular.z = _angularVelocityFactor;
                    //transform.Rotate(new Vector3(0, 0, 1) * deltaTime * 90 * 2 * (int)preyComponent.Speed);
                    //GetComponent<Raycast>().UpdateRays(deltaTime * 90 * 2);
                    //GetComponent<Raycast>().UpdateRays();
                }
                
                if (arrowUp)
                {
                    // move up
                    //translation.Value += forward * deltaTime * 2 * (int)preyComponent.Speed;
                    velocity.Linear = forward  * _linearVelocityFactor;
                    //transform.Translate(Vector2.up * deltaTime * 2 * (int)preyComponent.Speed);
                    //GetComponent<Raycast>().UpdateRays(0);
                    //GetComponent<Raycast>().UpdateRays();
                }
                if (arrowDown)
                {
                    // move down
                    //float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
                    //translation.Value += -forward * deltaTime * 2 * (int)preyComponent.Speed;
                    velocity.Linear = -forward * _linearVelocityFactor;
                    //transform.Translate(Vector2.down * deltaTime * 2 * (int)preyComponent.Speed);
                    //GetComponent<Raycast>().UpdateRays(0);
                    //GetComponent<Raycast>().UpdateRays();
                }
                if ((!arrowUp && !arrowRight && !arrowLeft && !arrowDown))
                {
                    if (preyComponent.Energy <= 100)
                        preyComponent.Energy = math.min(preyComponent.Energy + _energyGain * deltaTime, 100);
                }
                else
                {
                    preyComponent.Energy -= _energyGain * deltaTime;
                }
            }
            else 
            {
                DynamicBuffer<float> _outputs = outputs.Reinterpret<float>(); //brain.FeedForward(_inputs);

                float angularVelocity = _outputs[0];
                float linearVelocity = _outputs[1];

                if ((angularVelocity == 0f && linearVelocity == 0f))
                {
                    if (preyComponent.Energy <= 100)
                        preyComponent.Energy = math.min(preyComponent.Energy + _energyGain * deltaTime, 100);
                }
                else
                {
                    preyComponent.Energy -= _energyGain * deltaTime * math.max(math.abs(angularVelocity), math.abs(linearVelocity));
                }

                // if (float.IsNaN(translation.Value.x))
                // {
                //     //Debug.Log("translation is NaN");
                //     return;
                // }
                // float3 forwardMove = forward * 2 * math.min(preyComponent.Speed, 3f) * linearVelocity;
                // if (float.IsNaN(forwardMove.x) || float.IsNaN(forwardMove.y) || float.IsNaN(forwardMove.z) || float.IsNaN(deltaTime * 90 * 2 * (int)preyComponent.Speed * angularVelocity))
                // {
                //     //Debug.Log("forwardMove is NaN: " + forwardMove + " deltaTime: " + deltaTime + " angularVelocity: " + angularVelocity);
                //     return;
                // }
                //Debug.Log("forward " + forward + " forwardMove " + forwardMove + " deltaTime " + deltaTime + " angularVelocity " + angularVelocity + " linearVelocity " + linearVelocity + " preyComponent.Speed " + preyComponent.Speed);
                velocity.Linear = forward * _linearVelocityFactor * linearVelocity;
                velocity.Angular.z = _angularVelocityFactor * angularVelocity;
                //translation.Value += forward * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity;
                //rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(deltaTime * 90 * math.min((int)preyComponent.Speed, 3) * angularVelocity)));
            }
            // for (int i = 0; i < preyComponent._numRays; i++)
            // {
            //     _inputs[i * 2] = GetComponent<Raycast>().Distances[i];
            // }
            // for (int i = 0; i < _numRays; i++)
            // {
            //     _inputs[i * 2 + 1] = GetComponent<Raycast>().WhoIsThere[i];
            // }
            
            

            

            //float3 forward = math.mul(rotation.Value, new float3(0, 1, 0));
            //Debug.Log("linearVelocity: " + linearVelocity + " forward " + forward  + " deltatime " +  deltaTime + " speed: " + (int)preyComponent.Speed);
            //Debug.Log("linearVelocity update: " + forward * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity + " non-vector: " + deltaTime * 2 * (int)preyComponent.Speed * linearVelocity);
            //Debug.Log("angularVelocity: " + angularVelocity);
            // translation.Value += forward * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity;
            // rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(deltaTime * 90 * math.min((int)preyComponent.Speed, 3) * angularVelocity)));
            

            //transform.Translate(Vector2.up * deltaTime * 2 * (int)preyComponent.Speed * linearVelocity);
            //transform.Rotate(new Vector3(0, 0, angularVelocity) * deltaTime * 90 * 2 * (int)preyComponent.Speed);

            //GetComponent<Raycast>().UpdateRays();

        }).WithBurst().ScheduleParallel();//.WithBurst().ScheduleParallel();
        //Dependency.Complete();

        EntityCommandBuffer ecb = commandBufferSystem.CreateCommandBuffer();//new EntityCommandBuffer(Allocator.TempJob);
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);//commandBufferSystem.CreateCommandBuffer();
        //EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();
        Entities
        .WithAll<PreyTag>()
        .WithoutBurst()
        .ForEach((Entity e, ref Translation translation, ref PreyComponent preyComponent, ref DynamicBuffer<LayerDataElement> layers, ref DynamicBuffer<NeuronDataElement> neurons, ref DynamicBuffer<BiasDataElement> biases, ref DynamicBuffer<WeightDataElement> weights) => {
            // if (preyComponent.Alive == false)
            // {
            //     parallelEcb.DestroyEntity(entityInQueryIndex, e);
            //     SceneInitializerECS.NUMPREY--;
            //     return;
            // }
            if (preyComponent.Alive == false)
            {
                // Debug.Log("prey died " + EntityManager.Exists(e));
                // //parallelEcb.DestroyEntity(entityInQueryIndex, e);
                // ecb.DestroyEntity(e);
                // SceneInitializerECS.NUMPREY--;
                return;
            }
            if (preyComponent.Alive && (int)preyComponent._age % _timerReproduction == 0 && (math.floor(preyComponent._age) / _timerReproduction) > preyComponent._counterReproduction && SceneInitializerECS.NUMPREY < SceneInitializerECS.MAXPREYALLOWED)
            {
                preyComponent._counterReproduction++;
                //preyComponent._age = time;
                //Reproduce(translation, preyComponent.Generation, biases, weights, parallelEcb, entityInQueryIndex);

                // reproduce function
                if (SceneInitializerECS.NUMPREY == SceneInitializerECS.MAXPREYALLOWED) return;
                float2 posChild = random.NextFloat2(0, 1);
                float2 randomPosition = new float2(translation.Value.x, translation.Value.y) + posChild;
                //Vector2 randomPosition = new Vector2(translation.Value.x, translation.Value.y) + UnityEngine.Random.insideUnitCircle;
                float randomRotationAngle = random.NextFloat(0, 360); //UnityEngine.Random.Range(0.0f, 360.0f);
                quaternion randomRotation = quaternion.Euler(0.0f, 0.0f, math.radians(randomRotationAngle));
                //print("Random rotation " + randomRotationAngle);

                // Entity child = parallelEcb.CreateEntity(entityInQueryIndex, SceneInitializerECS.PreyArchetype);
                // parallelEcb.SetComponent(entityInQueryIndex, child, new Translation { Value = new float3(randomPosition.x, randomPosition.y, 0) });
                // parallelEcb.SetComponent(entityInQueryIndex, child, new Rotation { Value = randomRotation });

                Entity child = ecb.CreateEntity(SceneInitializerECS.PreyArchetype);
                ecb.SetComponent(child, new Translation { Value = new float3(randomPosition.x, randomPosition.y, 0) });
                ecb.SetComponent(child, new Rotation { Value = randomRotation });

                //child.GetComponent<Prey>().Start();
                //child.name = "Prey";

                //parallelEcb.SetComponent<PreyComponent>(entityInQueryIndex, child, new PreyComponent {
                ecb.SetComponent(child, new PreyComponent {
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
                    _timerStopCollision = 0f

                });

                //parallelEcb.SetComponent(entityInQueryIndex, child, new RaycastComponent() { 
                ecb.SetComponent(child, new RaycastComponent() { 
                        //physicsWorld = buildPhysicsWorld.PhysicsWorld,
                        _numberOfRays = numRaysPreys,
                        _fov = fovPreys,
                        _viewRange = viewRangePreys,
                        toggleShowRays = false,
                        _step = (float)fovPreys / (float)(numRaysPreys - 1)
                });
                // DynamicBuffer<DistanceDataElement> distancesChild = parallelEcb.AddBuffer<DistanceDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<WhoIsThereDataElement> whoIsThereChild = parallelEcb.AddBuffer<WhoIsThereDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<LayerDataElement> layersChild = parallelEcb.AddBuffer<LayerDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<NeuronDataElement> neuronsChild = parallelEcb.AddBuffer<NeuronDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<WeightDataElement> weightsChild = parallelEcb.AddBuffer<WeightDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<BiasDataElement> biasesChild = parallelEcb.AddBuffer<BiasDataElement>(entityInQueryIndex, child);
                // DynamicBuffer<OutputDataElement> outputsChild = parallelEcb.AddBuffer<OutputDataElement>(entityInQueryIndex, child);

                DynamicBuffer<DistanceDataElement> distancesChild = ecb.AddBuffer<DistanceDataElement>(child);
                DynamicBuffer<WhoIsThereDataElement> whoIsThereChild = ecb.AddBuffer<WhoIsThereDataElement>(child);
                DynamicBuffer<LayerDataElement> layersChild = ecb.AddBuffer<LayerDataElement>(child);
                DynamicBuffer<NeuronDataElement> neuronsChild = ecb.AddBuffer<NeuronDataElement>(child);
                DynamicBuffer<WeightDataElement> weightsChild = ecb.AddBuffer<WeightDataElement>(child);
                DynamicBuffer<BiasDataElement> biasesChild = ecb.AddBuffer<BiasDataElement>(child);
                DynamicBuffer<OutputDataElement> outputsChild = ecb.AddBuffer<OutputDataElement>(child);
                distancesChild.ResizeUninitialized(numRaysPreys);
                whoIsThereChild.ResizeUninitialized(numRaysPreys);
                weightsChild.ResizeUninitialized(numWeightsPreys);
                biasesChild.ResizeUninitialized(numNeuronsPreys);
                layersChild.CopyFrom(layers);
                outputsChild.ResizeUninitialized(2);
                //int[] brainStructure = BrainStructure();
                //layersChild.Reinterpret<int>().CopyFrom(brainStructurePreys);
                //DynamicBuffer<float> neurons = ecb.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
                
                neuronsChild.ResizeUninitialized(numNeuronsPreys);
                //DynamicBuffer<BiasDataElement> biasesChild = ecb.GetBuffer<BiasDataElement>(child);
                biases.CopyFrom(biases);
                //DynamicBuffer<WeightDataElement> weightsChild = ecb.GetBuffer<WeightDataElement>(child);
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

                //parallelEcb.SetSharedComponent(entityInQueryIndex, child, new RenderMesh() { 
                ecb.SetSharedComponent(child, new RenderMesh() { 
                    mesh = meshAgent,
                    material = materialPrey,
                    layerMask = 1
                });

                //parallelEcb.SetComponent(entityInQueryIndex, child, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});
                ecb.SetComponent(child, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});

                //parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsMass {
                ecb.SetComponent(child, new PhysicsMass {
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
                // var collisionFilter = new Unity.Physics.CollisionFilter
                // {
                //     BelongsTo = ~0u,
                //     CollidesWith = ~0u,
                //     GroupIndex = 0
                // };


                //parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
                ecb.SetComponent(child, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
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

                //parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsDamping()
                ecb.SetComponent(child, new PhysicsDamping()
                {
                    Linear = 4f,
                    Angular = 4f
                });

                //parallelEcb.SetComponent(entityInQueryIndex, child, new PhysicsGravityFactor()
                ecb.SetComponent(child, new PhysicsGravityFactor()
                {
                    Value = 0f
                });
                SceneInitializerECS.NUMPREY++;
                
            }

            if (preyComponent.Alive && preyComponent._energyExhausted)
            {
                // ecb.SetSharedComponent(e, new RenderMesh {
                //     mesh = ecb.GetSharedComponentData<RenderMesh>(e).mesh,
                //     material = ecb.GetSharedComponentData<RenderMesh>(e).material
                // });//.material.SetColor("_Color", new Color(0, 0.3f, 0));
                
                //parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 0.3f, 0, 1)});
                ecb.SetComponent(e, new URPMaterialPropertyBaseColor {Value = new float4(0, 0.3f, 0, 1)});
            }
            else if (preyComponent.Alive)
            {
                //preyComponent.Energy += 5f * deltaTime;
                //if (preyComponent.Energy >= 100f)
                //{
                    
                    //ecb.GetSharedComponentData<RenderMesh>(e).material.SetColor("_Color", new Color(0, 1f, 0));
                    //parallelEcb.SetComponent(entityInQueryIndex, e, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});
                    ecb.SetComponent(e, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});
                //}
            }
            
        }).Run();
        //Dependency.Complete();
        //ecb.Playback(EntityManager);
        //ecb.Dispose();
        commandBufferSystem.AddJobHandleForProducer(Dependency);//.Complete();
        //Debug.Log("Prey count: " + SceneInitializerECS.NUMPREY);
        //ecb.Playback(this.EntityManager);
        //ecb.Dispose();
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
