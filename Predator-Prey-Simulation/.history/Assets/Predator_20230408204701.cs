using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    private NeuralNetwork brain;
    // private Raycast[] inputs;
    public int Lifepoints { public get; private set; }
    public double Fitness { public get; private set; }
    private bool Alive { public get; private set; }

    [SerializeField] private float Energy { public get; private set; }
    [SerializeField] private double Speed { public get; private set; }

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
            print("I hit aPredator");
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
