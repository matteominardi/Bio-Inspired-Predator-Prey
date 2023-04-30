using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IComponent
{
    Transform transform { get; }
    GameObject gameObject { get; }
}

public interface ISelectable : IComponent
{
    int Lifepoints { get; }
    double Fitness { get; }
    bool Alive { get; }
    float Energy { get; }
    float Speed { get; }
    public int Generation { get; }
    public Raycast Raycast { get; }
    public int[] BrainModel { get; }
    public NeuralNetwork Brain { get; }

}

public class Prey : MonoBehaviour, ISelectable
{
    public static int MaxPrey = 1000;
    public static int Counter = 0;
    private readonly object _lockPreys = new object();
    private NeuralNetwork brain;
    public NeuralNetwork Brain { get => brain; }
    // private Raycast[] inputs;
    public int Lifepoints { get; private set; }
    public double Fitness { get; private set; }
    public bool Alive { get; private set; }

    public float Energy { get; private set; }
    public float Speed { get; private set; }
    public int Generation { get; private set; }
    public Raycast Raycast { get; private set; }
    public int[] BrainModel { get; private set; }



    private bool _energyExhausted;

    private float _age;
    private float[] _inputs;
    private float[] _outputs;
    //private bool guard = true;
    private int _counterReproduction = 0;
    private int _dmg = 5;
    private int _numRays = 24;
    private int _fov = 300;
    private int _viewRange = 30;

    // Start is called before the first frame update
    void Start()
    {
        // print(name);
        // Brain = new NeuralNetwork(new[] { 24, 5, 3 });
        // Lifepoints = 100;
        // Fitness = 0;
        // Alive = true;
        // Energy = 100;
        // Speed = 2;
        // Generation = 1;
        // _energyExhausted = false;
        // _age = Time.time;

        // GetComponent<Raycast>().Generate(24, 300, 30);
    }

    public void Generate(int generation, float speed, int dmg, int fov, int numRays, int viewRange, NeuralNetwork parent = null)//, float rotationAngle=0)
    {
        // lock (_lockPreys)
        // {
        // if (!CanReproduce.CanPreysReproduce())
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        //print("NUMBER OF PREYS " + CanReproduce.PreysCounter());
        Speed = speed;
        _dmg = dmg;
        _numRays = numRays;
        _fov = fov;
        _viewRange = viewRange;
        BrainModel = new int[] { _numRays * 2, 5, 2 };

        if (parent == null)
            brain = new NeuralNetwork(BrainModel);
        else
        {
            brain = parent;
            brain.Mutate(SceneInitializer.mutationRate, SceneInitializer.mutationAmount);
        }
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        //Speed = 2;
        Generation = generation;
        _energyExhausted = false;
        _age = Time.time;
        _inputs = new float[_numRays * 2];
        name = "Prey";
        gameObject.tag = "Prey";

        GetComponent<SpriteRenderer>().color = Color.green;
        //GetComponent<Raycast>().Generate(24, 90, 30, this);//, rotationAngle);
        this.Raycast = GetComponent<Raycast>();
        this.Raycast.Generate(_numRays, _fov, _viewRange, this);//, rotationAngle);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Raycast>().UpdateRays();
        if (!Alive)
            return;

        // if (Fitness % 15 > 5)
        //     guard = true;


        // if (Random.Range(0, 1000) < 10)
        // {
        //     //CanReproduce.DecrementPreysCounter();
        //     Counter--;
        //     Destroy(gameObject);
        // }
        // else 
        // {
        //     Reproduce(brain, Generation);
        // }

        Fitness += 1.0 * Time.deltaTime;

        // if ((Time.time - _age) > 10 && CanReproduce)
        // if ((Time.time - _age) > 10 && CanReproduce.CanPreysReproduce())
        // if ((Time.time - _age) > 10 && Counter < MaxPrey)
        if ((int)Fitness % 15 == 0 && (Math.Floor(Fitness) / 15) > _counterReproduction && Counter < MaxPrey)
        {
            // lock (_lockPreys)
            // {
            //guard = false;
            _counterReproduction++;
            _age = Time.time;
            Reproduce(brain, Generation);
            // }
        }

        if (Energy <= 0f)
        {
            //GetComponent<Raycast>().UpdateRays(0);
            GetComponent<Raycast>().UpdateRays();
            Energy += 5f * Time.deltaTime;
            _energyExhausted = true;
            GetComponent<SpriteRenderer>().color = new Color(0, 0.3f, 0);
            return;
        }

        if (Energy < 100f && _energyExhausted)
        {
            Energy += 5f * Time.deltaTime;
            if (Energy >= 100f)
            {
                Energy = 100f;
                _energyExhausted = false;
                GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            }
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // move right
            //transform.Translate(Vector2.right * Time.deltaTime * 2 );

            //rotate
            transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * 90 * 2 * (int)Speed);
            //GetComponent<Raycast>().UpdateRays(-Time.deltaTime * 90 * 2 * 0);
            GetComponent<Raycast>().UpdateRays();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // move left
            //transform.Translate(-Vector2.right * Time.deltaTime * 2 );

            //rotate
            transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 90 * 2 * (int)Speed);
            //GetComponent<Raycast>().UpdateRays(Time.deltaTime * 90 * 2);
            GetComponent<Raycast>().UpdateRays();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // move up
            transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed);
            //GetComponent<Raycast>().UpdateRays(0);
            GetComponent<Raycast>().UpdateRays();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // move down
            transform.Translate(Vector2.down * Time.deltaTime * 2 * (int)Speed);
            //GetComponent<Raycast>().UpdateRays(0);
            GetComponent<Raycast>().UpdateRays();
        }
        // if (!(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow)))
        // {
        //     if (Energy <= 100)
        //         Energy = Mathf.Min(Energy + 5f * Time.deltaTime, 100);
        // }
        // else
        // {
        //     Energy -= 10f * Time.deltaTime;
        // }

        for (int i = 0; i < _numRays; i++)
        {
            _inputs[i * 2] = GetComponent<Raycast>().Distances[i];
        }
        for (int i = 0; i < _numRays; i++)
        {
            _inputs[i * 2 + 1] = GetComponent<Raycast>().WhoIsThere[i];
        }
        _outputs = brain.FeedForward(_inputs);

        float angularVelocity = _outputs[0];
        float linearVelocity = _outputs[1];

        if (!(angularVelocity != 0f || linearVelocity != 0f || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            if (Energy <= 100)
                Energy = Mathf.Min(Energy + 5f * Time.deltaTime, 100);
        }
        else
        {
            Energy -= 5f * Time.deltaTime;
        }

        transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed * linearVelocity);
        transform.Rotate(new Vector3(0, 0, angularVelocity) * Time.deltaTime * 90 * 2 * (int)Speed);

        GetComponent<Raycast>().UpdateRays();
    }

    // void LateUpdate()
    // {
    //     for (int i = 0; i < 24; i++)
    //     {
    //         _inputs[i * 2] = GetComponent<Raycast>().Distances[i];
    //     }
    //     for (int i = 0; i < 24; i++)
    //     {
    //         _inputs[i * 2 + 1] = GetComponent<Raycast>().WhoIsThere[i];
    //     }
    //     _outputs = brain.FeedForward(_inputs);

    //     float angularVelocity = _outputs[0];
    //     float linearVelocity = _outputs[1];

    //     if (!(angularVelocity != 0f || linearVelocity != 0f || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow)))
    //     {
    //         if (Energy <= 100)
    //             Energy = Mathf.Min(Energy + 5f * Time.deltaTime, 100);
    //     }
    //     else
    //     {
    //         Energy -= 10f * Time.deltaTime;
    //     }

    //     transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed * linearVelocity);
    //     transform.Rotate(new Vector3(0, 0, angularVelocity) * Time.deltaTime * 90 * 2);

    //     GetComponent<Raycast>().UpdateRays();

    // }

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
        //     //GetComponent<Rigidbody2D>().AddForce(dir * 40);
        // }


        if (collision.gameObject.name == "Predator")
        {
            //print("I hit a Predator");
            Lifepoints -= _dmg;
            Fitness -= 50 * Time.deltaTime;

            if (Lifepoints <= 0)
            {
                // lock (_lockPreys)
                // {
                //print("I am dead");
                Alive = false;
                //CanReproduce.DecrementPreysCounter();
                Counter--;
                Lifepoints = 0;
                Destroy(gameObject);
                // }
            }
        }
        else if (collision.gameObject.name == "Prey")
        {
            //print("I hit a Prey");
            Lifepoints = Mathf.Min(Lifepoints + 1, 100);
            //Fitness += 0.25;
        }
    }

    void Reproduce(NeuralNetwork parent, int generation)
    {
        if (Counter == MaxPrey) return;
        Vector2 randomPosition = new Vector2(transform.position.x, transform.position.y) + UnityEngine.Random.insideUnitCircle;
        float randomRotationAngle = UnityEngine.Random.Range(0.0f, 360.0f);
        Quaternion randomRotation = Quaternion.Euler(0.0f, 0.0f, randomRotationAngle);
        //print("Random rotation " + randomRotationAngle);
        GameObject child = Instantiate(gameObject, randomPosition, randomRotation);

        //child.GetComponent<Prey>().Start();
        //child.name = "Prey";

        child.GetComponent<Prey>().Generate(generation + 1, Speed, _dmg, _fov, _numRays, _viewRange, parent.Copy(new NeuralNetwork(BrainModel)));//, randomRotationAngle);
        Counter++;
        // child.GetComponent<Prey>().Generation = generation;
        // child.GetComponent<Prey>().Speed = 2;
        //child.GetComponent<Prey>().Brain = parent.Copy(new NeuralNetwork(new[] { 24, 5, 3 }));
        // child.GetComponent<Prey>().Brain.Mutate(20, 0.5f);

        // child.GetComponent<Prey>().GetComponent<Raycast>().Generate(24, 300, 30);
    }

    public void SaveMyBrain(string path)
    {
        print("saving at " + path + "/PreyBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
        brain.Save(path + "/PreyBrain" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt");
    }

    public void LoadMyBrain(string path)
    {
        brain.Load(path);
    }
}
