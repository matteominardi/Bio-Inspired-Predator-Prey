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
    private int _viewRange;
    public float[] Distances;

    [SerializeField] private LayerMask layerMask;

    public void Generate(int _numberOfRays, int _fov, int _viewRange, float rotationAngle=0)
    {
        this._numberOfRays = _numberOfRays;
        this._fov = _fov;
        this._viewRange = _viewRange;
        this._rays = new Vector3[_numberOfRays];
        this._angles = new float[_numberOfRays];
        this.Distances = new float[_numberOfRays];
        Physics2D.queriesStartInColliders = false;
        layerMask = ~(1 << 6);

        float initAngle = (90f + rotationAngle) % 360f - (float)_fov / 2f;
        float step = (float)_fov / (_numberOfRays - 1);
        //print("Init angle " + initAngle + " step " + step.ToString("n2"));
        for (int i = 0; i < _numberOfRays; i++)
        {
            float angle = i * step + initAngle; // - _fov/2; //+ (_fov/_numberOfRays)/2;
            float angleInRadians = angle * Mathf.Deg2Rad;
            //print("i " + i +  " angle " + angle );
            _rays[i] = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            _angles[i] = angle;
            //Debug.DrawRay(transform.position, _rays[i] * _viewRange, Color.white, 30f);
        }
    }

    public void UpdateRays(float ang)
    {
        var pos = transform.position;

        for (int i = 0; i < _numberOfRays; i++)
        {
            _rays[i] = Quaternion.AngleAxis(ang, transform.forward) * _rays[i];
            _angles[i] += ang;

            RaycastHit2D hit = Physics2D.Raycast(pos, _rays[i], _viewRange, layerMask);

            Color color = Color.green;
            float rayLength = _viewRange;
            if (hit.collider != null)
            {
                // print(
                //     "I can see a " + hit.collider.gameObject.name + " at " + hit.distance + " units"
                // );
                rayLength = hit.distance;
                
            }
            else
            {
                //print("I hit nothing");
                color = Color.gray;
            }
            Distances[i] = rayLength < _viewRange ? 1/rayLength : 0;
            Debug.DrawRay(pos, _rays[i].normalized * rayLength, color);
        }
    }
}
