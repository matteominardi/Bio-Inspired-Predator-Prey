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

        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<Raycast>().Generate(24, 90, 60);

    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive) 
            return;
        
        Fitness += 1.0 * Time.deltaTime;
        GetComponent<Raycast>().UpdateRays(0);

        if (Energy <= 0)
        {
            print("I am dead");
            Alive = false;
            Fitness = -1;
            Destroy(gameObject);
        }
        else
        {
            Energy -= 1.0f * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Prey" || collision.gameObject.name == "Predator") {
            // Calculate Angle Between the collision point and the player
            Vector2 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody2D>().AddForce(dir * 16);
        }
        
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
