using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Predator : MonoBehaviour, ISelectable
{
    public static int MaxPredator = 32;
    public static int Counter = 0;
    private NeuralNetwork brain;
    public NeuralNetwork Brain { get => brain; }

    // private Raycast[] inputs;
    public int Lifepoints { get; private set; }
    public double Fitness { get; private set; }
    public bool Alive { get; private set; }

    public float Energy { get; private set; }
    public double Speed { get; private set; }
    public int Generation { get; private set; }
    public float ReproductionFactor { get; private set; }
    public Raycast Raycast { get; private set; }
    public int[] BrainModel { get; private set; }

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

    public void Generate(int generation, NeuralNetwork parent = null)//, float rotationAngle = 0)
    {
        // if (!CanReproduce.CanPredatorsReproduce())
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        BrainModel = new int[] { 48, 5, 2 };
        
        if (parent == null)
            brain = new NeuralNetwork(BrainModel);
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
        _inputs = new float[48];
        name = "Predator";
        gameObject.tag = "Predator";



        GetComponent<SpriteRenderer>().color = Color.red;
        this.Raycast = GetComponent<Raycast>();
        this.Raycast.Generate(24, 90, 60, this);//, rotationAngle);

    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive)
            return;

        // if (ReproductionFactor >= 100 && CanReproduce)
        // if (ReproductionFactor >= 100 && CanReproduce.CanPredatorsReproduce())
        if (ReproductionFactor >= 100 && Counter < MaxPredator)
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
            //print("I am dead");
            Alive = false;
            Fitness = -1;
            Counter--;
            Destroy(gameObject);
        }
        else
        {
            Energy -= 1.0f * Time.deltaTime;
        }
    }

    void LateUpdate()
    {

        for (int i = 0; i < 24; i++)
        {
            _inputs[i * 2] = GetComponent<Raycast>().Distances[i];
        }
        for (int i = 0; i < 24; i++)
        {
            _inputs[i * 2 + 1] = GetComponent<Raycast>().WhoIsThere[i];
            //print("i " + i + " " + "whosthere " + _inputs[i*2+1]);
        }
        // for (int i = 0; i < _inputs.Length; i++)
        // {
        //     print("Whosthere " + i*2 + " "+ (i*2 + 1) + " : " + GetComponent<Raycast>().WhoIsThere[i] + "  " + _inputs.Length + " " +i);
        //     _inputs[i*2] = GetComponent<Raycast>().Distances[i];
        //     _inputs[i*2+1] = GetComponent<Raycast>().WhoIsThere[i];
        // }
        _outputs = brain.FeedForward(_inputs);

        float angularVelocity = _outputs[0];// * 2 - 1;
        float linearVelocity = _outputs[1];// * 2 - 1;

        transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed * linearVelocity);
        transform.Rotate(new Vector3(0, 0, angularVelocity) * Time.deltaTime * 90 * 2);

        //print("Angular Velocity: " + angularVelocity);
        //print("Linear Velocity: " + linearVelocity);

        //GetComponent<Raycast>().UpdateRays(Time.deltaTime * 90 * 2 * (angularVelocity));
        GetComponent<Raycast>().UpdateRays();


    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // if (collision.gameObject.name == "Prey" || collision.gameObject.name == "Predator")
        // {
        //     // Calculate Angle Between the collision point and the player
        //     Vector2 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
        //     // We then get the opposite (-Vector3) and normalize it
        //     dir = -dir.normalized;
        //     // And finally we add force in the direction of dir and multiply it by force. 
        //     // This will push back the player
        //     GetComponent<Rigidbody2D>().AddForce(dir * 40);
        // }

        if (collision.gameObject.name == "Predator")
        {
            //print("I hit a Predator");
        }
        else if (collision.gameObject.name == "Prey")
        {
            //print("I hit a Prey");
            Lifepoints -= 5;
            //Fitness += 2;

            if (collision.gameObject.GetComponent<Prey>().Lifepoints <= 0)
            {
                Energy += 30;
                Lifepoints = Mathf.Min(Lifepoints + 50, 100);
                ReproductionFactor += 100f;
                //Fitness += 10;
            }

            if (Lifepoints <= 0)
            {
                //print("I am dead");
                Alive = false;
                Fitness = -1;
                CanReproduce.DecrementPredatorsCounter();
                Destroy(gameObject);
            }
        }
    }

    void Reproduce(NeuralNetwork parent, int generation)
    {
        if (Counter == MaxPredator) return;
        Vector2 randomPosition = new Vector2(transform.position.x, transform.position.y) + UnityEngine.Random.insideUnitCircle;
        float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
        //print("Predator Random rotation " + randomRotationAngle);
        Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
        GameObject child = Instantiate(gameObject, randomPosition, randomRotation);

        //child.GetComponent<Prey>().Start();
        //child.name = "Prey";

        child.GetComponent<Predator>().Generate(generation + 1, parent.Copy(new NeuralNetwork(new[] { 48, 5, 3 })));//, randomRotationAngle);
        Counter++;

        // child.GetComponent<Prey>().Generation = generation;
        // child.GetComponent<Prey>().Speed = 2;
        //child.GetComponent<Prey>().Brain = parent.Copy(new NeuralNetwork(new[] { 24, 5, 3 }));
        // child.GetComponent<Prey>().Brain.Mutate(20, 0.5f);

        // child.GetComponent<Prey>().GetComponent<Raycast>().Generate(24, 300, 30);
    }


    public void SaveMyBrain() 
    {
        brain.Save("./Assets/PretrainedNetworks/PredatorBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
    }

    public void LoadMyBrain(string path)
    {
        brain.Load(path);
    }
}
