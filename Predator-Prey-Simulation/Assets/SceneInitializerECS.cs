using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;


public class SceneInitializerECS : MonoBehaviour
{
    [SerializeField] public Mesh meshAgent;
    [SerializeField] public UnityEngine.Material materialPrey;
    [SerializeField] public UnityEngine.Material materialPredator;

    public static Mesh MeshAgent;
    public static UnityEngine.Material MaterialPrey;
    public static UnityEngine.Material MaterialPredator;
    public static EntityArchetype PreyArchetype;
    public static EntityArchetype PredatorArchetype;
    public Prey preyPrefab;
    public Predator predatorPrefab;
    private float _time;
    public static int[] _brainStructurePreys = new[] { 48, 5, 2 };
    public static int[] _brainStructurePredators = new[] { 48, 5, 2 };
    public GameObject Background;

    public static int NUMPREY = 4000;
    public static int NUMPREDATOR = 4000;
    public static int MAXPREYALLOWED = 6000;
    public static int MAXPREDATORALLOWED = 6000;
    public static bool loadPretrained = false;
    public static int mapSize = 80; // scale is 1:10 (1 unit here = 5 units in the world) (eg. mapSize = 40 => 200x200 world)
    public static int timerReproductionPreys = 5;
    public static float energyGainPreys = 5.0f;
    public static float energyLossPredators = 1.0f;
    public static float reprodGainWhenEatPredators = 30f;
    public static float speedPreys = 3f;
    public static float speedPredators = 2f;
    public static int dmgPreys = 15;
    public static int dmgPredators = 50;
    public static int fovPreys = 300;
    public static int fovPredators = 90;
    public static int numRaysPreys = 24;
    public static int numRaysPredators = 24;
    public static int viewRangePreys = 60;
    public static int viewRangePredators = 120;
    public static float mutationRate = 0.2f;
    public static float mutationAmount = 0.5f;
    public static float deviationAmount = 0.5f;
    public static string path;
    public static bool ManualMovement = false;

    private EntityManager entityManager;
    //private static SceneInitializerECS instance;

    // Start is called before the first frame update
    void Awake()
    {
        //print("dev " + deviationAmount + " timerrepr preys " + timerReproductionPreys + " energyGainPreys " + energyGainPreys + " energyLossPredators " + energyLossPredators + " reprodGainWhenEatPredators " + reprodGainWhenEatPredators);
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else if (instance != this)
        // {
        //     Destroy(gameObject);
        // }
        //print("Awake");
        Application.targetFrameRate = -1;
        # if UNITY_EDITOR
        path = "Assets/PretrainedNetworks/";
        # else
        path = Path.Combine(Application.persistentDataPath, "Assets/PretrainedNetworks/");
        # endif
        MeshAgent = meshAgent;
        MaterialPrey = materialPrey;
        MaterialPredator = materialPredator;
        // Prey firstPrey = Instantiate<Prey>(preyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // firstPrey.Generate(1);

        // Predator firstPredator = Instantiate<Predator>(predatorPrefab, new Vector3(4, 0, 0), Quaternion.identity);
        // firstPredator.Generate(1);
        _time = Time.time;
        _brainStructurePreys[0] = numRaysPreys * 2;
        _brainStructurePredators[0] = numRaysPredators * 2;

        Prey.MaxPrey = MAXPREYALLOWED;
        Predator.MaxPredator = MAXPREDATORALLOWED;

        Background.transform.localScale = new Vector3(mapSize, 1, mapSize);
        Background.GetComponent<Renderer>().material.mainTextureScale = new Vector2(mapSize, mapSize);

        GameObject leftWall = GameObject.Find("leftWall");
        GameObject rightWall = GameObject.Find("rightWall");
        GameObject topWall = GameObject.Find("topWall");
        GameObject bottomWall = GameObject.Find("bottomWall");

        leftWall.transform.position = new Vector3(-mapSize * 5 - leftWall.transform.localScale.x / 2, 0, 0);
        rightWall.transform.position = new Vector3(mapSize * 5 + rightWall.transform.localScale.x / 2, 0, 0);
        topWall.transform.position = new Vector3(0, mapSize * 5 + topWall.transform.localScale.y / 2, 0);
        bottomWall.transform.position = new Vector3(0, -mapSize * 5 - bottomWall.transform.localScale.y / 2, 0);

        leftWall.transform.localScale = new Vector3(leftWall.transform.localScale.x, mapSize * 10 + 20, 1);
        rightWall.transform.localScale = new Vector3(rightWall.transform.localScale.x, mapSize * 10 + 20, 1);
        topWall.transform.localScale = new Vector3(mapSize * 10 + 20, topWall.transform.localScale.y, 1);
        bottomWall.transform.localScale = new Vector3(mapSize * 10 + 20, bottomWall.transform.localScale.y, 1);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        meshAgent = SceneInitializerECS.MeshAgent;
        materialPrey = SceneInitializerECS.MaterialPrey;
        //_random.InitState(120);// = new Unity.Mathematics.Random(1);
        //uint _layerMask = ~((1 << 6) | (1 << 5));
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

        EntityArchetype archetypePrey = entityManager.CreateArchetype(
            typeof(PreyTag),
            typeof(Translation),
            typeof(Rotation),
            typeof(PreyComponent),
            typeof(RaycastComponent),
            typeof(DistanceDataElement),
            typeof(WhoIsThereDataElement),
            typeof(LayerDataElement),
            typeof(NeuronDataElement),
            typeof(WeightDataElement),
            typeof(BiasDataElement),
            typeof(OutputDataElement),
            typeof(PhysicsCollider),
            typeof(PhysicsVelocity),
            //typeof(PhysicsMass),
            //typeof(PhysicsShape),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(URPMaterialPropertyBaseColor),
            typeof(RenderBounds),
            typeof(PhysicsMass),
            typeof(PhysicsWorldIndex),
            typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor)
            //typeof()
        );
        PreyArchetype = archetypePrey;

        EntityArchetype archetypePredator = entityManager.CreateArchetype(
            typeof(PredatorTag),
            typeof(Translation),
            typeof(Rotation),
            typeof(PredatorComponent),
            typeof(RaycastComponent),
            typeof(DistanceDataElement),
            typeof(WhoIsThereDataElement),
            typeof(LayerDataElement),
            typeof(NeuronDataElement),
            typeof(WeightDataElement),
            typeof(BiasDataElement),
            typeof(OutputDataElement),
            typeof(PhysicsCollider),
            typeof(PhysicsVelocity),
            //typeof(PhysicsMass),
            //typeof(PhysicsShape),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(URPMaterialPropertyBaseColor),
            typeof(RenderBounds),
            typeof(PhysicsMass),
            typeof(PhysicsWorldIndex),
            typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor)
            //typeof()
        );
        PredatorArchetype = archetypePredator;


        //NeuralNetwork netPrey = null;
        //NeuralNetwork netPredator = null;
        bool isNetPreyLoaded = false;
        bool isNetPredatorLoaded = false;
        // DynamicBuffer<BiasDataElement>      biasesPreyLoaded        = new DynamicBuffer<BiasDataElement>();
        // DynamicBuffer<BiasDataElement>      biasesPredatorLoaded     = new DynamicBuffer<BiasDataElement>();
        // DynamicBuffer<WeightDataElement>    weightsPreyLoaded      = new DynamicBuffer<WeightDataElement>();
        // DynamicBuffer<WeightDataElement>    weightsPredatorLoaded  = new DynamicBuffer<WeightDataElement>();
       

        //int[] brainStructurePreys = _brainStructurePreys;
        int numNeuronsPreys = 0;
        int numWeightsPreys = 0;
        for (int j = 0; j < _brainStructurePreys.Length; j++)
        {
            numNeuronsPreys += _brainStructurePreys[j];
        }
        for (int j = 0; j < _brainStructurePreys.Length-1; j++)
        {
            numWeightsPreys += _brainStructurePreys[j] * _brainStructurePreys[j+1];
        }
        //print("numNeuronsPreys: " + numNeuronsPreys + " numWeightsPreys: " + numWeightsPreys);
        //int[] brainStructurePredators = _brainStructurePreys;
        int numNeuronsPredators = 0;
        int numWeightsPredators = 0;
        for (int j = 0; j < _brainStructurePredators.Length; j++)
        {
            numNeuronsPredators += _brainStructurePredators[j];
        }
        for (int j = 0; j < _brainStructurePredators.Length-1; j++)
        {
            numWeightsPredators += _brainStructurePredators[j] * _brainStructurePredators[j+1];
        }
        float[]   biasesPreyLoaded       = new float[numNeuronsPreys];
        float[]   biasesPredatorLoaded   = new float[numNeuronsPredators];
        float[]   weightsPreyLoaded      = new float[numWeightsPreys];
        float[]   weightsPredatorLoaded  = new float[numWeightsPredators];
    
        if (loadPretrained)
        {
            //netPrey = new NeuralNetwork(_brainStructurePreys);
            //netPredator = new NeuralNetwork(_brainStructurePredators);
            // biasesPreyLoaded      .ResizeUninitialized(numNeuronsPreys);
            // biasesPredatorLoaded  .ResizeUninitialized(numNeuronsPredators);
            // weightsPreyLoaded     .ResizeUninitialized(numWeightsPreys);
            // weightsPredatorLoaded .ResizeUninitialized(numWeightsPredators);
            string[] paths = GetPathsOfMostRecentBrains(); // first element is prey, second is predator
            if (paths[0] != null)
            {
                //print(paths[0]);
                LoadNeuralNetwork(paths[0], biasesPreyLoaded, weightsPreyLoaded, numNeuronsPreys, numWeightsPreys);
                //netPrey.Load(paths[0]); //on start load the network save
                isNetPreyLoaded = true;
                // for (int i = 0; i < 48; i++)
                // {
                //     print("Prey: " + i + " " + netPrey[0,0,i]);
                // }
            }
            if (paths[1] != null)
            {
                //print(paths[1]);
                //netPrey.Load(paths[0]); //on start load the network save
                LoadNeuralNetwork(paths[1], biasesPredatorLoaded, weightsPredatorLoaded, numNeuronsPredators, numWeightsPredators);
                isNetPredatorLoaded = true;

            }


        }


        for (int i = 0; i < NUMPREY; i++)
        { 
            var entity = entityManager.CreateEntity(archetypePrey);
            float3 randomPos = new float3(UnityEngine.Random.Range(-SceneInitializerECS.mapSize * 5 + 2, SceneInitializerECS.mapSize * 5 - 2), UnityEngine.Random.Range(-SceneInitializerECS.mapSize * 5 + 2, SceneInitializerECS.mapSize * 5 - 2), 0f);
            float randomRotation = UnityEngine.Random.Range(0f, 360f);
            //EntityManager.SetComponentData(entity, new Translation { Value = new float3(_random.NextFloat(-3f, 3f), _random.NextFloat(-3f, 3f), 0f) });
            entityManager.SetComponentData(entity, new Translation { Value = randomPos });
            entityManager.SetComponentData(entity, new Rotation { Value = quaternion.Euler(0f, 0f, math.radians(randomRotation)) }); //_random.NextFloat(0f, 0f) math.radians(180f)
            entityManager.SetComponentData(entity, new PreyComponent() { 
                //Brain = new NeuralNetworkComponent(),
                // private Raycast[] inputs;
                Lifepoints = 100,
                Fitness = 0f,
                Alive = true,

                Energy = 100f,
                Speed = speedPreys,
                Generation = 0,
                //public Raycast Raycast
                //public DynamicBuffer<IntDataElement> BrainModel;



                _energyExhausted = false,

                _age = 0,
                //public DynamicBuffer<FloatDataElement> _inputs;
                //public DynamicBuffer<FloatDataElement> _outputs;
                //public bool guard = true;
                _counterReproduction = 0,
                _dmg = dmgPreys,
                _timerStopCollision = 0f
                // _numRays;
                // public int _fov;
                // public int _viewRange;
            });

            

            entityManager.SetComponentData(entity, new RaycastComponent() { 
                //physicsWorld = buildPhysicsWorld.PhysicsWorld,
                _numberOfRays = numRaysPreys,
                _fov = fovPreys,
                _viewRange = viewRangePreys,
                toggleShowRays = false
            });
            entityManager.AddBuffer<DistanceDataElement>(entity);
            entityManager.AddBuffer<WhoIsThereDataElement>(entity);
            entityManager.AddBuffer<LayerDataElement>(entity);
            entityManager.AddBuffer<NeuronDataElement>(entity);
            entityManager.AddBuffer<WeightDataElement>(entity);
            entityManager.AddBuffer<BiasDataElement>(entity);
            entityManager.AddBuffer<OutputDataElement>(entity);

            entityManager.GetBuffer<DistanceDataElement>(entity).ResizeUninitialized(numRaysPreys);
            entityManager.GetBuffer<WhoIsThereDataElement>(entity).ResizeUninitialized(numRaysPreys);
            
            //int[] brainStructure = BrainStructure();
            entityManager.GetBuffer<LayerDataElement>(entity).Reinterpret<int>().CopyFrom(_brainStructurePreys);
            //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
            
            entityManager.GetBuffer<NeuronDataElement>(entity).ResizeUninitialized(numNeuronsPreys);
            if (isNetPreyLoaded)
            {
                entityManager.GetBuffer<WeightDataElement>(entity).Reinterpret<float>().CopyFrom(weightsPreyLoaded);
                entityManager.GetBuffer<BiasDataElement>(entity).Reinterpret<float>().CopyFrom(biasesPreyLoaded);
                // DynamicBuffer<float> weights = entityManager.GetBuffer<WeightDataElement>(entity).Reinterpret<float>();
                // for (int j = 0; j < entityManager.GetBuffer<WeightDataElement>(entity).Length; j++)
                // {
                //     Debug.Log("Init weight " + j + " " + weights[j]);
                // }
                // DynamicBuffer<float> biases = entityManager.GetBuffer<BiasDataElement>(entity).Reinterpret<float>();
                // for (int j = 0; j < entityManager.GetBuffer<BiasDataElement>(entity).Length; j++)
                // {
                //     Debug.Log("Init bias "+ j + " "+ biases[j]);
                // }
            }
            else 
            {
                DynamicBuffer<WeightDataElement> weights = entityManager.GetBuffer<WeightDataElement>(entity);
                DynamicBuffer<BiasDataElement> biases = entityManager.GetBuffer<BiasDataElement>(entity);
                InitWeights(weights, _brainStructurePreys);
                InitBiases(biases, _brainStructurePreys);
            }
            

            //InitNeurons(neurons, brainStructure);
            

            entityManager.SetSharedComponentData(entity, new RenderMesh() { 
                mesh = meshAgent,
                material = materialPrey,
                layerMask = 1,
            });

            entityManager.SetComponentData(entity, new URPMaterialPropertyBaseColor {Value = new float4(0, 1f, 0, 1)});

            entityManager.SetComponentData(entity, new PhysicsMass {
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


            entityManager.SetComponentData(entity, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
                Center = new float3(0f, 0f, 0f),
                Radius = 0.5f,

            }, CollisionFilter.Default, material) });

            // entityManager.SetComponentData(entity, new PhysicsCollider { Value = Unity.Physics.CylinderCollider.Create(new CylinderGeometry() { 
            //     Center = new float3(0f, 0f, 0f),
            //     Radius = 0.5f,
            //     Height = 1f,
            //     Orientation = quaternion.Euler(0f, 0f, math.radians(0f)),
            //     SideCount = 3
            // }, Unity.Physics.CollisionFilter.Default) });

            entityManager.SetComponentData(entity, new PhysicsDamping()
            {
                Linear = 4f,
                Angular = 4f
            });

            entityManager.SetComponentData(entity, new PhysicsGravityFactor()
            {
                Value = 0f
            });
        }

        for (int i = 0; i < NUMPREDATOR; i++)
        { 
            var entity = entityManager.CreateEntity(archetypePredator);
            float3 randomPos = new float3(UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), 0f);
            float randomRotation = UnityEngine.Random.Range(0f, 360f);
            //EntityManager.SetComponentData(entity, new Translation { Value = new float3(_random.NextFloat(-3f, 3f), _random.NextFloat(-3f, 3f), 0f) });
            entityManager.SetComponentData(entity, new Translation { Value = randomPos });
            entityManager.SetComponentData(entity, new Rotation { Value = quaternion.Euler(0f, 0f, math.radians(randomRotation)) }); //_random.NextFloat(0f, 0f) math.radians(180f)
            entityManager.SetComponentData(entity, new PredatorComponent() { 
                //Brain = new NeuralNetworkComponent(),
                // private Raycast[] inputs;
                Lifepoints = 100,
                Fitness = 0f,
                Alive = true,

                Energy = 100f,
                Speed = 2f,
                Generation = 0,
                ReproductionFactor = 0f,
                //public Raycast Raycast
                //public DynamicBuffer<IntDataElement> BrainModel;



                //_energyExhausted = false,

                //_age = 0,
                //public DynamicBuffer<FloatDataElement> _inputs;
                //public DynamicBuffer<FloatDataElement> _outputs;
                //public bool guard = true;
                //_counterReproduction = 0,
                _dmg = dmgPredators,
                _timerStopCollision = 0f
                // _numRays;
                // public int _fov;
                // public int _viewRange;
            });

            

            entityManager.SetComponentData(entity, new RaycastComponent() { 
                //physicsWorld = buildPhysicsWorld.PhysicsWorld,
                _numberOfRays = numRaysPredators,
                _fov = fovPredators,
                _viewRange = viewRangePredators,
                toggleShowRays = false
            });
            entityManager.AddBuffer<DistanceDataElement>(entity);
            entityManager.AddBuffer<WhoIsThereDataElement>(entity);
            entityManager.AddBuffer<LayerDataElement>(entity);
            entityManager.AddBuffer<NeuronDataElement>(entity);
            entityManager.AddBuffer<WeightDataElement>(entity);
            entityManager.AddBuffer<BiasDataElement>(entity);
            entityManager.AddBuffer<OutputDataElement>(entity);

            entityManager.GetBuffer<DistanceDataElement>(entity).ResizeUninitialized(numRaysPredators);
            entityManager.GetBuffer<WhoIsThereDataElement>(entity).ResizeUninitialized(numRaysPredators);
            //int[] brainStructure = BrainStructure();
            entityManager.GetBuffer<LayerDataElement>(entity).Reinterpret<int>().CopyFrom(_brainStructurePredators);
            //DynamicBuffer<float> neurons = EntityManager.GetBuffer<NeuronDataElement>(entity).Reinterpret<float>();
            
            entityManager.GetBuffer<NeuronDataElement>(entity).ResizeUninitialized(numNeuronsPredators);
            if (isNetPredatorLoaded)
            {
                entityManager.GetBuffer<WeightDataElement>(entity).Reinterpret<float>().CopyFrom(weightsPredatorLoaded);
                entityManager.GetBuffer<BiasDataElement>(entity).Reinterpret<float>().CopyFrom(biasesPredatorLoaded);
            }
            else 
            {
                DynamicBuffer<WeightDataElement> weights = entityManager.GetBuffer<WeightDataElement>(entity);
                DynamicBuffer<BiasDataElement> biases = entityManager.GetBuffer<BiasDataElement>(entity);
                InitWeights(weights, _brainStructurePredators);
                InitBiases(biases, _brainStructurePredators);
            }
            

            //InitNeurons(neurons, brainStructure);
            

            entityManager.SetSharedComponentData(entity, new RenderMesh() { 
                mesh = meshAgent,
                material = materialPredator,
                layerMask = 1
            });

            entityManager.SetComponentData(entity, new URPMaterialPropertyBaseColor {Value = new float4(1f, 0, 0, 1)});

            entityManager.SetComponentData(entity, new PhysicsMass {
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


            entityManager.SetComponentData(entity, new PhysicsCollider { Value = Unity.Physics.SphereCollider.Create(new SphereGeometry() { 
                Center = new float3(0f, 0f, 0f),
                Radius = 0.5f,


            }, CollisionFilter.Default, material) });

            entityManager.SetComponentData(entity, new PhysicsDamping()
            {
                Linear = 4f,
                Angular = 4f
            });

            entityManager.SetComponentData(entity, new PhysicsGravityFactor()
            {
                Value = 0f
            });
        }


        // for (int i = 0; i < NUMPREY; i++)
        // {
        //     float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
        //     Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
            
            //entityManager.Instantiate(preys[i]);
            //Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), 0), randomRotation);
            //Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(0,0,0), Quaternion.identity);
            // if (loadPretrained && isNetPreyLoaded)
            // {
            //     prey.Generate(1, speedPreys, dmgPreys, fovPreys, numRaysPreys, viewRangePreys, netPrey);
            // }
            // else
            // {
            //     prey.Generate(1, speedPreys, dmgPreys, fovPreys, numRaysPreys, viewRangePreys);
            // }
        // }
        // Prey.Counter = NUMPREY;
        // for (int i = 0; i < NUMPREY; i++)
        // {
        //     float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
        //     Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
        //     Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), 0), randomRotation);
        //     //Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(0,0,0), Quaternion.identity);
        //     if (loadPretrained && isNetPreyLoaded)
        //     {
        //         prey.Generate(1, speedPreys, dmgPreys, fovPreys, numRaysPreys, viewRangePreys, netPrey);
        //     }
        //     else
        //     {
        //         prey.Generate(1, speedPreys, dmgPreys, fovPreys, numRaysPreys, viewRangePreys);
        //     }
        // }
        // Prey.Counter = NUMPREY;

        // for (int i = 0; i < NUMPREDATOR; i++)
        // {
        //     float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
        //     Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
            // Predator predator = Instantiate<Predator>(predatorPrefab, new Vector3(UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), UnityEngine.Random.Range(-mapSize * 5 + 2, mapSize * 5 - 2), 0), randomRotation);
            // if (loadPretrained && isNetPredatorLoaded)
            // {
            //     predator.Generate(1, speedPredators, dmgPredators, fovPredators, numRaysPredators, viewRangePredators, netPredator);
            // }
            // else
            // {
            //     predator.Generate(1, speedPredators, dmgPredators, fovPredators, numRaysPredators, viewRangePredators);
            // }
        // }
        // Predator.Counter = NUMPREDATOR;
    }

    string[] GetPathsOfMostRecentBrains()
    {
        string directoryPath = path;//Path.Combine(Application.persistentDataPath, "Assets/PretrainedNetworks");
        //print(directoryPath);
        Directory.CreateDirectory(directoryPath);

        string[] filesPredator = Directory.GetFiles(directoryPath, "PredatorBrain*.txt");
        string[] filesPrey = Directory.GetFiles(directoryPath, "PreyBrain*.txt");

        string mostRecentPredatorFile = filesPredator
            .OrderByDescending(f => DateTime.ParseExact(
                Path.GetFileNameWithoutExtension(f).Substring("PredatorBrain".Length),
                "dd-MM-yyyy",
                null))
            .FirstOrDefault();

        string mostRecentPreyFile = filesPrey
            .OrderByDescending(f => DateTime.ParseExact(
                Path.GetFileNameWithoutExtension(f).Substring("PreyBrain".Length),
                "dd-MM-yyyy",
                null))
            .FirstOrDefault();

        string[] paths = new string[2];
        paths[0] = mostRecentPreyFile;
        paths[1] = mostRecentPredatorFile;
        return paths;
    }



    // Update is called once per frame
    // void Update()
    // {
    //     int numPrey = GameObject.FindGameObjectsWithTag("Prey").Length;
    //     int numPredator = GameObject.FindGameObjectsWithTag("Predator").Length;

    //     if (numPrey > 100) {
    //         Prey.CanReproduce = false;
    //     } else {
    //         Prey.CanReproduce = true;
    //     }

    //     if (numPredator > 10) {
    //         Predator.CanReproduce = false;
    //     } else {
    //         Predator.CanReproduce = true;
    //     }
    // }

    // void Update()
    // {
    //     int numPrey = GameObject.FindGameObjectsWithTag("Prey").Length;
    //     int numPredator = GameObject.FindGameObjectsWithTag("Predator").Length;

    //     if (numPrey > ) {
    //         print("ROTTO " + numPrey);
    //     } 
    // }

    void Update()
    {
        // if (Time.time - _time > 10)
        // {
        //     _time = Time.time;
        //     GameObject[] preys = GameObject.FindGameObjectsWithTag("Prey");
        //     GameObject[] predators = GameObject.FindGameObjectsWithTag("Predator");

        //     if (preys.Length > 0)
        //     {
        //         GameObject bestPrey = preys[0];
        //         foreach (GameObject prey in preys)
        //         {
        //             if (prey.GetComponent<Prey>().Fitness > bestPrey.GetComponent<Prey>().Fitness)
        //             {
        //                 bestPrey = prey;
        //             }
        //         }
        //         bestPrey.GetComponent<Prey>().SaveMyBrain(Path.Combine(Application.persistentDataPath, "Assets/PretrainedNetworks"));
        //     }


        //     if (predators.Length > 0)
        //     {
        //         GameObject bestPredator = predators[0];
        //         foreach (GameObject predator in predators)
        //         {
        //             if (predator.GetComponent<Predator>().Fitness > bestPredator.GetComponent<Predator>().Fitness)
        //             {
        //                 bestPredator = predator;
        //             }
        //         }
        //         bestPredator.GetComponent<Predator>().SaveMyBrain(Path.Combine(Application.persistentDataPath, "Assets/PretrainedNetworks"));
        //     }
        // }
    }

    public static int[] BrainStructure()
    {
        return new int[3] { numRaysPreys > numRaysPredators ? numRaysPreys * 2 : numRaysPredators * 2, 5, 2 };

    }

    private void InitBiases(DynamicBuffer<BiasDataElement> biases, int[] layers)
    {
        //List<float[]> biasList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++)
        {
            //float[] bias = new float[layers[i]];

            for (int j = 0; j < layers[i]; j++)
            {
                //bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
                biases.Add(new BiasDataElement { Value = UnityEngine.Random.Range(-deviationAmount, deviationAmount)});
            }

            //biasList.Add(bias);
        }

        //biases = biasList.ToArray();
    }

    private void InitWeights(DynamicBuffer<WeightDataElement> weights, int[] layers)
    {
        //List<float[][]> WeightsList = new List<float[][]>();

        for (int i = 0; i < layers.Length - 1; i++)
        {
            //List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInNextLayer = layers[i + 1];

            for (int j = 0; j < layers[i]; j++)
            {
                //float[] neuronWeights = new float[neuronsInNextLayer];

                for (int k = 0; k < neuronsInNextLayer; k++)
                {
                    //neuronWeights[k] = UnityEngine.Random.Range(-0.01f, 0.01f);
                    weights.Add(new WeightDataElement { Value =  UnityEngine.Random.Range(-deviationAmount, deviationAmount)});
                }

                //layerWeightsList.Add(neuronWeights);
            }

            //WeightsList.Add(layerWeightsList.ToArray());
        }

        //_weights = WeightsList.ToArray();
    }

    public void LoadNeuralNetwork(string path, float[] _biases, float[] _weights, int numBiases, int numWeights)
    {
        //DynamicBuffer<float> _biases = biases.Reinterpret<float>();
        //DynamicBuffer<float> _weights = weights.Reinterpret<float>();
        StreamReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        List<string> ListLines = new List<string>();
        int index = 0;


        while (!tr.EndOfStream)
        {
            ListLines.Add(tr.ReadLine());
        }
        //string[] ArrayLines = ListLines.ToArray();
        //Debug.Log("Number of lines: " + ListLines.Count);
        //print("Number of biases: " + numBiases);
        //print("Number of weights: " + numWeights);
        // for (int i = 0; i < NumberOfLines; i++)
        // {

        //     ListLines[i] = tr.ReadLine();

        //     Debug.Log("LINE " + i + " :" + ListLines[i]);
        // }

        tr.Close();

        if ((new FileInfo(path)).Length > 0)
        {
            //Debug.Log("numbiases " + numBiases);
            for (int i = 0; i < numBiases; i++)
            {
                //for (int j = 0; j < _biases[i].Length; j++)
                //{
                    _biases[i] = float.Parse(ListLines[index]);
                    //Debug.Log("bias " + i + " : " + _biases[i]);
                    index++;
                //}
            }
            //print("index: " + index + " value: " + ListLines[index]);

            for (int i = 0; i < numWeights; i++)
            {
                //for (int j = 0; j < _weights[i].Length; j++)
                //{
                    //for (int k = 0; k < _weights[i][j].Length; k++)
                    //{
                        //print("index: " + index + " value: " + ListLines[index]);
                        _weights[i] = float.Parse(ListLines[index]);
                        index++;
                   // }
                //}
            }
        }
    }
}
