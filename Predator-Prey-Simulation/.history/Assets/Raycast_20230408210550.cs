using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    //[SerializeField] LayerMask layerMask;
    //private GameObject parent;
    private int numberOfRays = 24;
    private Vector3[] rays;
    private float[] angles;
    private int fov = 300;
    private int viewRange;

    public void Generate(int numberOfRays, int fov, int viewRange)
    {
        this.numberOfRays = numberOfRays;
        this.fov = fov;
        this.viewRange = viewRange;
        this.rays = new Vector3[numberOfRays];
        this.angles = new float[numberOfRays];

        Physics2D.queriesStartInColliders = false;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = i * (fov / numberOfRays) + 90 - fov / 2 + (fov / numberOfRays) / 2;
            float angleInRadians = angle * Mathf.Deg2Rad;
            rays[i] = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            angles[i] = angle;
            Debug.DrawRay(transform.position, rays[i] * viewRange, Color.white, 3f);
        }
    }

    public void UpdateRays(float ang)
    {
        var pos = transform.position;

        for (int i = 0; i < numberOfRays; i++)
        {
            rays[i] = Quaternion.AngleAxis(ang, transform.forward) * rays[i];
            angles[i] += ang;

            RaycastHit2D hit = Physics2D.Raycast(pos, rays[i] * viewRange, 10);

            Color color = Color.green;
            if (hit.collider != null)
            {
                print("I hit something " + hit.collider.gameObject.name);
            }
            else
            {
                //print("I hit nothing");
                color = Color.red;
            }
            Debug.DrawRay(pos, rays[i] * viewRange, color);
        }


    }
}
