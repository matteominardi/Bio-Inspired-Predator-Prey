using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position + new Vector3(0.51f,0,0);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.right*10, 10);
        

        if (hit.collider != null)
        {
            print("I hit something" + hit.collider.gameObject.name);
            Debug.DrawRay(pos, Vector2.right*10, Color.green);
        }
        else
        {
            print("I hit nothing");
            Debug.DrawRay(pos, Vector2.right*10, Color.red);
        }
    }
}
