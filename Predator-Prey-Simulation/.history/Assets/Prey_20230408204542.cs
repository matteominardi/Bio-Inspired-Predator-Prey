using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    private NeuralNetwork Brain;
    // private Raycast[] inputs;
    public int Lifepoints { get; private set; }
    public double Fitness { get; private set; }
    public bool Alive { public get; private set; }

    [SerializeField] private float Energy { public get; private set; }
    [SerializeField] private double Speed { public get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        print("Prey");
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        Speed = 2;

        GetComponent<Raycast>().Generate(24, 300, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            Fitness += 1.0;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                // move right
                //transform.Translate(Vector2.right * Time.deltaTime * 2 );

                //rotate
                transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * 90);

                GetComponent<Raycast>().UpdateRays(-Time.deltaTime * 90);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // move left
                //transform.Translate(-Vector2.right * Time.deltaTime * 2 );

                //rotate
                transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 90);
                GetComponent<Raycast>().UpdateRays(Time.deltaTime * 90);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // move up
                transform.Translate(Vector2.up * Time.deltaTime * 2);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                // move down
                transform.Translate(Vector2.down * Time.deltaTime * 2);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Predator")
        {
            print("I hit a Predator");
            Lifepoints -= 25;

            if (lifepoints <= 0)
            {
                print("I am dead");
                Alive = false;
                Fitness = -1;
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.name == "Prey")
        {
            print("I hit a Prey");
            Lifepoints += 5;
            Fitness += 0.25;
        }
    }

    public void UpdateFitness()
    {
        brain.Fitness = Fitness;
    }
}