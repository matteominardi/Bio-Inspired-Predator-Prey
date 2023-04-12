using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    int Lifepoints { get; }
    double Fitness { get; }
    bool Alive { get; }
    float Energy { get; }
    double Speed { get; }
    public int Generation { get; }

}

public class Prey : MonoBehaviour, ISelectable
{
    private NeuralNetwork Brain;
    // private Raycast[] inputs;
    public int Lifepoints { get; private set; }
    public double Fitness { get; private set; }
    public bool Alive { get; private set; }

    public float Energy { get; private set; }
    public double Speed { get; private set; }
    public int Generation { get; private set; }

    private bool energyExhausted = false;

    private float age;

    // Start is called before the first frame update
    public void Start()
    {
        print(name);
        Brain = new NeuralNetwork(new[]{24, 5, 3});
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        Speed = 2;
        Generation = 1;
        age = Time.time;

        GetComponent<Raycast>().Generate(24, 300, 30);
    }

    public void Generate(int generation) {
        Brain = new NeuralNetwork(new[]{24, 5, 3});
        Lifepoints = 100;
        Fitness = 0;
        Alive = true;
        Energy = 100;
        Speed = 2;
        Generation = generation;
        age = Time.time;

        GetComponent<Raycast>().Generate(24, 300, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Alive)
            return;
        
        Fitness += 1.0 * Time.deltaTime;

        if ((Time.time - age) > 10)
        {
            age = Time.time;
            Reproduce(Brain, Generation);
        }

        if (Energy <= 0f) 
        {
            GetComponent<Raycast>().UpdateRays(0);
            Energy += 5f * Time.deltaTime;
            energyExhausted = true;
            GetComponent<SpriteRenderer>().color = new Color(0, 0.3f, 0);
            return;
        }
        
        if (Energy < 10f && energyExhausted)
        {
            Energy += 5f * Time.deltaTime;
            if (Energy >= 10f) 
            {
                energyExhausted = false;
                GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            }
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // move right
            //transform.Translate(Vector2.right * Time.deltaTime * 2 );

            //rotate
            transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * 90 * 2);
            GetComponent<Raycast>().UpdateRays(-Time.deltaTime * 90 * 2);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // move left
            //transform.Translate(-Vector2.right * Time.deltaTime * 2 );

            //rotate
            transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 90 * 2);
            GetComponent<Raycast>().UpdateRays(Time.deltaTime * 90 * 2);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // move up
            transform.Translate(Vector2.up * Time.deltaTime * 2 * (int)Speed);
            GetComponent<Raycast>().UpdateRays(0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // move down
            transform.Translate(Vector2.down * Time.deltaTime * 2 * (int)Speed);
            GetComponent<Raycast>().UpdateRays(0);
        }
        if (!(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow))) {
            if (Energy <= 100)
                Energy = Mathf.Min(Energy + 5f * Time.deltaTime, 100);
        } else 
        {
            Energy -= 10f * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Calculate Angle Between the collision point and the player
        Vector2 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
        // We then get the opposite (-Vector3) and normalize it
        dir = -dir.normalized;
        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player
        GetComponent<Rigidbody2D>().AddForce(dir*4);

        if (collision.gameObject.name == "Predator")
        {
            print("I hit a Predator");
            Lifepoints -= 25;

            if (Lifepoints <= 0)
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
        Brain.Fitness = Fitness;
    }

    void Reproduce(NeuralNetwork parent, int generation)
    {
        GameObject child = Instantiate(gameObject, new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        child.SetActiveRecursively()
        child.name = "Prey";
        
        child.GetComponent<Prey>().Generate(generation + 1);

        // child.GetComponent<Prey>().Generation = generation;
        // child.GetComponent<Prey>().Speed = 2;
        child.GetComponent<Prey>().Brain = parent.Copy(new NeuralNetwork(new[]{24, 5, 3}));
        child.GetComponent<Prey>().Brain.Mutate(20, 0.5f);
        child.GetComponent<Prey>().Brain.Fitness = 0;

        child.GetComponent<Prey>().GetComponent<Raycast>().Generate(24, 300, 30);
    }
}
