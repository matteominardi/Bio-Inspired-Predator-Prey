using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour, ISelectable
{
    private NeuralNetwork brain;
    // private Raycast[] inputs;
    public int Lifepoints { get; private set; }
    public double Fitness { get; private set; }
    public bool Alive { get; private set; }

    public float Energy { get; private set; }
    public double Speed { get; private set; }
    public int Generation { get; private set; }


    void Start()
    {
        // print("Predator");
        // Lifepoints = 100;
        // Fitness = 0;
        // Alive = true;
        // Energy = 100;
        // Speed = 3;

        // GetComponent<Raycast>().Generate(24, 90, 60);
    }

    public void Generate(int generation, NeuralNetwork parent = null)
    {
        if (parent == null)
            brain = new NeuralNetwork(new[] { 24, 5, 3 });
        else
        {
            brain = parent;
            brain.Mutate(20, 0.5f);
        }
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        Speed = 3;
        Generation = generation;
        name = "Predator";

        GetComponent<Raycast>().Generate(24, 90, 60);

    }

    // Update is called once per frame
    void Update()
    {
        if (Alive) {
            Fitness += 1.0 * Time.deltaTime;
            GetComponent<Raycast>().UpdateRays(0);
        }
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
            Lifepoints -= 25;
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
}
