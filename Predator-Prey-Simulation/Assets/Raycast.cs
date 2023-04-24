using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    //[SerializeField] LayerMask layerMask;
    //private GameObject parent;
    private int _numberOfRays;
    private Vector3[] _rays;
    private float[] _angles;
    private int _fov;
    public int Fov { get { return _fov; } }
    private int _viewRange;
    public int ViewRange { get { return _viewRange; } }
    public float[] Distances;
    public float[] WhoIsThere;
    public bool toggleShowRays;

    [SerializeField] private LayerMask layerMask;

    public void Generate(
        int _numberOfRays,
        int _fov,
        int _viewRange,
        MonoBehaviour parentObj
        //float rotationAngle = 0
    )
    {
        this._numberOfRays = _numberOfRays;
        this._fov = _fov;
        this._viewRange = _viewRange;
        this._rays = new Vector3[_numberOfRays];
        this._angles = new float[_numberOfRays];
        this.Distances = new float[_numberOfRays];
        this.WhoIsThere = new float[_numberOfRays];
        this.transform.parent = parentObj.transform;
        this.toggleShowRays = false;
        Physics2D.queriesStartInColliders = false;
        layerMask = ~((1 << 6) | (1 << 5));

        // float initAngle = (90f + rotationAngle) % 360f - (float)_fov / 2f;
        // float step = (float)_fov / (_numberOfRays - 1);
        // //print("Init angle " + initAngle + " step " + step.ToString("n2"));
        // for (int i = 0; i < _numberOfRays; i++)
        // {
        //     float angle = i * step + initAngle; // - _fov/2; //+ (_fov/_numberOfRays)/2;
        //     float angleInRadians = angle * Mathf.Deg2Rad;
        //     //print("i " + i +  " angle " + angle );
        //     _rays[i] = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        //     _angles[i] = angle;
        //     //Debug.DrawRay(transform.position, _rays[i] * _viewRange, Color.white, 30f);
        // }
    }

    // public void UpdateRays(float ang)
    // {
    //     var pos = transform.position;

    //     for (int i = 0; i < _numberOfRays; i++)
    //     {
    //         _rays[i] = Quaternion.AngleAxis(ang, transform.forward) * _rays[i];
    //         _angles[i] += ang;

    //         RaycastHit2D hit = Physics2D.Raycast(pos, _rays[i], _viewRange, layerMask);

    //         Color color = Color.green;
    //         float rayLength = _viewRange;
    //         if (hit.collider != null)
    //         {
    //             // print(
    //             //     "I can see a " + hit.collider.gameObject.name + " at " + hit.distance + " units"
    //             // );
    //             rayLength = hit.distance;
    //         }
    //         else
    //         {
    //             //print("I hit nothing");
    //             color = Color.gray;
    //         }
    //         Distances[i] = rayLength < _viewRange ? 1 / rayLength : 0;
    //         Debug.DrawRay(pos, _rays[i].normalized * rayLength, color);
    //     }
    // }

    public void UpdateRays()
    {
        var pos = transform.position;

        for (int i = 0; i < _numberOfRays; i++)
        {
            //_rays[i] = Quaternion.AngleAxis(ang, transform.forward) * _rays[i];
            //_angles[i] += ang;

            float step = (float)_fov / (float)(_numberOfRays - 1);    
            // float angle = (i * step + (_fov / 2) + (360-_fov))%360;
            // //float angle = i * step + (- _fov / 2)%360;
            float cosineAngle = Mathf.Acos(Vector3.Dot(transform.up, Vector3.up) / (transform.up.magnitude * Vector3.up.magnitude));
            float angle = (float)((float)i * step  + cosineAngle) % 360.0f - (float)_fov/2.0f;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * transform.up;
            //if (i == 0)
            //print("i " + i + " angle " + angle.ToString("n2") + " dir " + dir + " transform.up " + transform.up + " angle of rotation " + Vector3.Angle(Vector3.up, transform.up));

            RaycastHit2D hit = Physics2D.Raycast(pos, dir, _viewRange, layerMask);

            Color color = Color.green;
            float rayLength = _viewRange;
            if (hit.collider != null)
            {
                // print(
                //     "I can see a " + hit.collider.gameObject.name + " at " + hit.distance + " units"
                // );
                rayLength = hit.distance;
                float whosThere = 0;
                if (hit.collider.gameObject.name == "Prey")
                {
                    whosThere = 1.0f;
                }
                else if (hit.collider.gameObject.name == "Predator")
                {
                    whosThere = -1.0f;
                    color = Color.red;

                }

                WhoIsThere[i] = whosThere;
            }
            else
            {
                //print("I hit nothing");
                color = Color.gray;
                WhoIsThere[i] = 0.0f;

            }
            Distances[i] = rayLength < _viewRange ? 1 / rayLength : 0;
            if (toggleShowRays)
                Debug.DrawRay(pos, dir.normalized * rayLength, color);
        }
    }
}
