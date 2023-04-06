using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("Prey");
    }

    // Update is called once per frame
    void Update()
    {

        // if right arrow key is pressed
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // move right
            transform.Translate(Vector2.right * Time.deltaTime * 2 );
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // move right
            transform.Translate(Vector2.up * Time.deltaTime * 2 );
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // move right
            transform.Translate(Vector2.down * Time.deltaTime * 2);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // move right
            transform.Translate(Vector2.left * Time.deltaTime * 2 );
        }
        // move right
        //transform.Translate(Vector2.right * Time.deltaTime );
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
        }
    }
}
