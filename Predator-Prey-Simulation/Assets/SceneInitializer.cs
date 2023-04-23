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

    // Start is called before the first frame update
    void Awake()
    {
        // Prey firstPrey = Instantiate<Prey>(preyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // firstPrey.Generate(1);

        // Predator firstPredator = Instantiate<Predator>(predatorPrefab, new Vector3(4, 0, 0), Quaternion.identity);
        // firstPredator.Generate(1);
        _time = Time.time;
        int NUMPREY = 3;
        int NUMPREDATOR = 3;
        bool loadPretrained = true;



        NeuralNetwork netPrey = null;
        NeuralNetwork netPredator = null;
        bool isNetPreyLoaded = false;
        bool isNetPredatorLoaded = false;

        if (loadPretrained)
        {
            netPrey = new NeuralNetwork(new[] { 48, 5, 2 });
            netPredator = new NeuralNetwork(new[] { 48, 5, 2 });
            string[] paths = GetPathsOfMostRecentBrains(); // first element is prey, second is predator
            if (paths[0] != null)
            {
                netPrey.Load(paths[0]); //on start load the network save
                isNetPreyLoaded = true;
                for (int i = 0; i < 48; i++)
                {
                    print("Prey: " + i + " " + netPrey[0,0,i]);
                }
            }
            if (paths[1] != null)
            {
                netPrey.Load(paths[0]); //on start load the network save
                isNetPredatorLoaded = true;

            }
            
            
        }


        for (int i = 0; i < NUMPREY; i++)
        {
            Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0), Quaternion.identity);
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
            Predator predator = Instantiate<Predator>(predatorPrefab, new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0), Quaternion.identity);
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
