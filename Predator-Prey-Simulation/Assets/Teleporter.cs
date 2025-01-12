using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private Transform _destination;
    public bool IsTop,
        IsBottom,
        IsLeft;
    public float Distance = 0.2f;

    private GameObject leftWall;
    private GameObject rightWall;
    private GameObject topWall;
    private GameObject bottomWall;

    void Start()
    {
        leftWall = GameObject.Find("leftWall");
        rightWall = GameObject.Find("rightWall");
        topWall = GameObject.Find("topWall");
        bottomWall = GameObject.Find("bottomWall");

        // print("leftWall: " + leftWall.transform.position);
        // print("rightWall: " + rightWall.transform.position);
        // print("topWall: " + topWall.transform.position);
        // print("bottomWall: " + bottomWall.transform.position);

        
        // if (IsTop)
        // {
        //     _destination = GameObject.FindGameObjectWithTag("BottomWall").GetComponent<Transform>();
        // }
        // else if (IsBottom)
        // {
        //     _destination = GameObject.FindGameObjectWithTag("TopWall").GetComponent<Transform>();
        // }
        // else if (IsLeft)
        // {
        //     _destination = GameObject.FindGameObjectWithTag("RightWall").GetComponent<Transform>();
        // }
        // else
        // {
        //     _destination = GameObject.FindGameObjectWithTag("LeftWall").GetComponent<Transform>();
        // }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //print("I hit something");
        //print(other.ClosestPoint(transform.position));
        Vector2 closestPoint = other.ClosestPoint(transform.position);
        // bottom wall
        //print("closestPoint.y: " + closestPoint.y + " bottomWall.transform.position.y: " + bottomWall.transform.position.y + " bottomWall.transform.localScale.y/2: " + bottomWall.transform.localScale.y/2 + "");
        if (closestPoint.y < bottomWall.transform.position.y + bottomWall.transform.localScale.y/2 + 1)
        {
            //print("I hit the bottom wall " + bottomWall.transform.position.y + 1 + " teleport to: " + new Vector2(other.gameObject.transform.position.x, topWall.transform.position.y + 1));
            other.gameObject.transform.position = new Vector2(other.gameObject.transform.position.x, topWall.transform.position.y - topWall.transform.localScale.y/2 - 1);
        }
        // left wall
        if (closestPoint.x < leftWall.transform.position.x + leftWall.transform.localScale.x/2 + 1)
        {
            other.gameObject.transform.position = new Vector2(rightWall.transform.position.x - rightWall.transform.localScale.x/2-1, other.gameObject.transform.position.y);
        }
        // top wall
        if (closestPoint.y > topWall.transform.position.y - topWall.transform.localScale.y/2 - 1)
        {
            other.gameObject.transform.position = new Vector2(other.gameObject.transform.position.x, bottomWall.transform.position.y + bottomWall.transform.localScale.y/2 + 1);
        }
        // right wall
        if (closestPoint.x > rightWall.transform.position.x - rightWall.transform.localScale.x/2 - 1)
        {
            other.gameObject.transform.position = new Vector2(leftWall.transform.position.x + leftWall.transform.localScale.x/2  + 1, other.gameObject.transform.position.y);
        }
        // if (Vector2.Distance(transform.position, other.transform.position) > Distance)
        // {
        //     other.transform.position = new Vector2(_destination.position.x, _destination.position.y);
        // }
    }

    // private GameObject leftWall;
    // private GameObject rightWall;
    // private GameObject topWall;
    // private GameObject bottomWall;
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.name == "Prey")
    //     {
    //         print("Collided with prey");
    //         //collision.gameObject.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
    //     }
    // }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     leftWall = GameObject.Find("LeftWall");
    //     rightWall = GameObject.Find("RightWall");
    //     topWall = GameObject.Find("TopWall");
    //     bottomWall = GameObject.Find("BottomWall");
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
