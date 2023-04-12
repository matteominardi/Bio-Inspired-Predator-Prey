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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
