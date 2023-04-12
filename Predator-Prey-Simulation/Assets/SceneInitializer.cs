using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public Prey preyPrefab;
    public Predator predatorPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        Prey firstPrey = Instantiate<Prey>(preyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        firstPrey.Generate(1);

        Predator firstPredator = Instantiate<Predator>(predatorPrefab, new Vector3(4, 0, 0), Quaternion.identity);
        firstPredator.Generate(1);

        for (int i = 0; i < 10; i++)
        {
            Prey prey = Instantiate<Prey>(preyPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), Quaternion.identity);
            prey.Generate(1);
        }

        for (int i = 0; i < 10; i++)
        {
            Predator predator = Instantiate<Predator>(predatorPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), Quaternion.identity);
            predator.Generate(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
