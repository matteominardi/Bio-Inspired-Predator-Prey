using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    private NeuralNetwork brain;
    // private Raycast[] inputs;
    [SerializeField] public int Lifepoints { get; private set; }
    [SerializeField] public double Fitness { get; private set; }
    [SerializeField] public bool Alive { get; private set; }

    [SerializeField] public float Energy { get; private set; }
    [SerializeField] public double Speed { get; private set; }

    void Start()
    {
        print("Predator");
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        Speed = 3;

        GetComponent<Raycast>().Generate(24, 90, 30);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Predator")
        {
            print("I hit a Predator");
        }
        else if (collision.gameObject.name == "Prey")
        {
            print("I hit a Prey");
            Lifepoints -= 10;
            Fitness += 2;

            if (collision.gameObject.GetComponent<Prey>().Lifepoints <= 0)
            {
                Energy += 10;
                Fitness += 10;
            }

            if (Lifepoints <= 0)
            {
                print("I am dead");
                Alive = false;
                Fitness = -1;
                Destroy(gameObject);
            }
        }
    }

    public void UpdateFitness()
    {
        brain.Fitness = Fitness;
    }
}
