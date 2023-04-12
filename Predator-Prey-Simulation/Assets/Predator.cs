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
    public float ReproductionFactor { get; private set; }
    private float[] _inputs;
    private float[] _outputs;


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

    public void Generate(int generation, NeuralNetwork parent = null, float rotationAngle = 0)
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
        ReproductionFactor = 0f;
        _inputs = new float[24];
        name = "Predator";

        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<Raycast>().Generate(24, 90, 60, rotationAngle);

    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive) 
            return;
        
        if (ReproductionFactor >= 100)
        {
            //print("I am reproducing");
            Reproduce(brain, Generation);
            ReproductionFactor = 0;
        }

        Fitness += 1.0 * Time.deltaTime;
        ReproductionFactor = Mathf.Max(ReproductionFactor - 1.0f * Time.deltaTime, 0);
        //GetComponent<Raycast>().UpdateRays(0);

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

    void LateUpdate() {
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = GetComponent<Raycast>().Distances[i];
        }
        _outputs = brain.FeedForward(_inputs);

        float angularVelocity = _outputs[0];// * 2 - 1;
        float linearVelocity = _outputs[1];// * 2 - 1;

        transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed * linearVelocity);
        transform.Rotate(new Vector3(0, 0, angularVelocity) * Time.deltaTime * 90 * 2);

        print("Angular Velocity: " + angularVelocity);
        print("Linear Velocity: " + linearVelocity);

        GetComponent<Raycast>().UpdateRays(Time.deltaTime * 90 * 2 * (angularVelocity));


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
            //Fitness += 2;

            if (collision.gameObject.GetComponent<Prey>().Lifepoints <= 0)
            {
                Energy += 30;
                ReproductionFactor += 50f;
                //Fitness += 10;
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

    void Reproduce(NeuralNetwork parent, int generation)
    {
        Vector2 randomPosition = new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle;
        float randomRotationAngle = Random.Range(0.0f, 360.0f);
        Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
        GameObject child = Instantiate(gameObject, randomPosition, randomRotation);

        //child.GetComponent<Prey>().Start();
        //child.name = "Prey";

        child.GetComponent<Predator>().Generate(generation + 1, parent.Copy(new NeuralNetwork(new[] { 24, 5, 3 })), randomRotationAngle);

        // child.GetComponent<Prey>().Generation = generation;
        // child.GetComponent<Prey>().Speed = 2;
        //child.GetComponent<Prey>().Brain = parent.Copy(new NeuralNetwork(new[] { 24, 5, 3 }));
        // child.GetComponent<Prey>().Brain.Mutate(20, 0.5f);

        // child.GetComponent<Prey>().GetComponent<Raycast>().Generate(24, 300, 30);
    }
}
