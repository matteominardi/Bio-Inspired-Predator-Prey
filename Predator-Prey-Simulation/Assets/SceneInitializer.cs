using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class SceneInitializer : MonoBehaviour
{
    public Prey preyPrefab;
    public Predator predatorPrefab;
    private float _time;
    private static int[] _brainStructure = new[] {48, 5, 2};
    public GameObject Background;
    
    public static int[] BrainStructure()
    {
        return _brainStructure;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Prey firstPrey = Instantiate<Prey>(preyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // firstPrey.Generate(1);

        // Predator firstPredator = Instantiate<Predator>(predatorPrefab, new Vector3(4, 0, 0), Quaternion.identity);
        // firstPredator.Generate(1);
        _time = Time.time;
        int NUMPREY = 500;
        int NUMPREDATOR = 500;
        int MAXPREYALLOWED = 500;
        int MAXPREDATORALLOWED = 500;
        bool loadPretrained = false;
        int mapSize = 40; // scale is 1:10 (1 unit here = 5 units in the world) (eg. mapSize = 40 => 200x200 world)

        Prey.MaxPrey = MAXPREYALLOWED;
        Predator.MaxPredator = MAXPREDATORALLOWED;

        Background.transform.localScale = new Vector3(mapSize, 1, mapSize);
        Background.GetComponent<Renderer>().material.mainTextureScale = new Vector2(mapSize, mapSize);

        GameObject leftWall = GameObject.Find("leftWall");
        GameObject rightWall = GameObject.Find("rightWall");
        GameObject topWall = GameObject.Find("topWall");
        GameObject bottomWall = GameObject.Find("bottomWall");

        leftWall.transform.position = new Vector3(-mapSize*5-leftWall.transform.localScale.x/2, 0, 0);
        rightWall.transform.position = new Vector3(mapSize*5+rightWall.transform.localScale.x/2, 0, 0);
        topWall.transform.position = new Vector3(0, mapSize*5+topWall.transform.localScale.y/2, 0);
        bottomWall.transform.position = new Vector3(0, -mapSize*5-bottomWall.transform.localScale.y/2, 0);

        leftWall.transform.localScale = new Vector3(leftWall.transform.localScale.x, mapSize*10+20, 1);
        rightWall.transform.localScale = new Vector3(rightWall.transform.localScale.x, mapSize*10+20, 1);
        topWall.transform.localScale = new Vector3(mapSize*10+20, topWall.transform.localScale.y, 1);
        bottomWall.transform.localScale = new Vector3(mapSize*10+20, bottomWall.transform.localScale.y, 1);



        NeuralNetwork netPrey = null;
        NeuralNetwork netPredator = null;
        bool isNetPreyLoaded = false;
        bool isNetPredatorLoaded = false;

        if (loadPretrained)
        {
            netPrey = new NeuralNetwork(BrainStructure());
            netPredator = new NeuralNetwork(BrainStructure());
            string[] paths = GetPathsOfMostRecentBrains(); // first element is prey, second is predator
            if (paths[0] != null)
            {
                netPrey.Load(paths[0]); //on start load the network save
                isNetPreyLoaded = true;
                // for (int i = 0; i < 48; i++)
                // {
                //     print("Prey: " + i + " " + netPrey[0,0,i]);
                // }
            }
            if (paths[1] != null)
            {
                netPrey.Load(paths[0]); //on start load the network save
                isNetPredatorLoaded = true;

            }
            
            
        }


        for (int i = 0; i < NUMPREY; i++)
        {
            float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
            Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
            Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(UnityEngine.Random.Range(-mapSize*5+2, mapSize*5-2), UnityEngine.Random.Range(-mapSize*5+2, mapSize*5-2), 0), randomRotation);
            //Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(0,0,0), Quaternion.identity);
            if (loadPretrained && isNetPreyLoaded)
            {
                prey.Generate(1, netPrey);
            }
            else
            {
                prey.Generate(1);
            }
        }
        Prey.Counter = NUMPREY;

        for (int i = 0; i < NUMPREDATOR; i++)
        {
            float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
            Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
            Predator predator = Instantiate<Predator>(predatorPrefab, new Vector3(UnityEngine.Random.Range(-mapSize*5+2, mapSize*5-2), UnityEngine.Random.Range(-mapSize*5+2, mapSize*5-2), 0), randomRotation);
            if (loadPretrained && isNetPredatorLoaded)
            {
                predator.Generate(1, netPredator);
            }
            else
            {
                predator.Generate(1);
            }
        }
        Predator.Counter = NUMPREDATOR;
    }

    string[] GetPathsOfMostRecentBrains() 
    {
        string directoryPath = "./Assets/PretrainedNetworks";

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
        if (Time.time - _time > 10) {
            _time = Time.time;
            GameObject[] preys = GameObject.FindGameObjectsWithTag("Prey");
            GameObject[] predators = GameObject.FindGameObjectsWithTag("Predator");

            if (preys.Length > 0) 
            {
                GameObject bestPrey = preys[0];
                foreach (GameObject prey in preys) 
                {
                    if (prey.GetComponent<Prey>().Fitness > bestPrey.GetComponent<Prey>().Fitness) {
                        bestPrey = prey;
                    }
                }
                bestPrey.GetComponent<Prey>().SaveMyBrain();
            }
            

            if (predators.Length > 0) 
            {
                GameObject bestPredator = predators[0];
                foreach (GameObject predator in predators) 
                {
                    if (predator.GetComponent<Predator>().Fitness > bestPredator.GetComponent<Predator>().Fitness) {
                        bestPredator = predator;
                    }
                }
                bestPredator.GetComponent<Predator>().SaveMyBrain();
            }
        }
    }
}
