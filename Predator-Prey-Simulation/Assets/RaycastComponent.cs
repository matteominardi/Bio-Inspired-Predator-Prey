using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using System.Runtime.InteropServices;

[InternalBufferCapacity(100)]
public struct DistanceDataElement : IBufferElementData
{
    public float Value;
}

[InternalBufferCapacity(100)]
public struct WhoIsThereDataElement : IBufferElementData
{
    public float Value;
}

// public struct DistanceBuffer : IBufferElementData
// {
//     public DynamicBuffer<DistanceDataElement> Value;
// }

// public struct WhoIsThereBuffer : IBufferElementData
// {
//     public DynamicBuffer<WhoIsThereDataElement> Value;
// }


public struct RaycastComponent : IComponentData
{
    //[SerializeField] LayerMask layerMask;
    //private GameObject parent;
    //public PhysicsWorld physicsWorld;
    public int _numberOfRays;
    ////public Vector3[] _rays;
    ////public float[] _angles;
    public int _fov;
    //public int Fov { get { return _fov; } }
    public int _viewRange;
    ////public int ViewRange { get { return _viewRange; } }
    //public float[] Distances;
    //public float[] WhoIsThere;
    public bool toggleShowRays;
    public float _step;

    //[SerializeField] private LayerMask layerMask;

    // public void Generate(
    //     int _numberOfRays,
    //     int _fov,
    //     int _viewRange,
    //     MonoBehaviour parentObj
    //     //float rotationAngle = 0
    // )
    // {
    //     this._numberOfRays = _numberOfRays;
    //     this._fov = _fov;
    //     this._viewRange = _viewRange;
    //     this._rays = new Vector3[_numberOfRays];
    //     this._angles = new float[_numberOfRays];
    //     this.Distances = new float[_numberOfRays];
    //     this.WhoIsThere = new float[_numberOfRays];
    //     this.transform.parent = parentObj.transform;
    //     this.toggleShowRays = false;
    //     Physics2D.queriesStartInColliders = false;
    //     layerMask = ~((1 << 6) | (1 << 5));
    // }

    // public void UpdateRays()
    // {
    //     var pos = transform.position;

    //     for (int i = 0; i < _numberOfRays; i++)
    //     {

    //         float step = (float)_fov / (float)(_numberOfRays - 1);    
    //         float cosineAngle = Mathf.Acos(Vector3.Dot(transform.up, Vector3.up) / (transform.up.magnitude * Vector3.up.magnitude));
    //         float angle = (float)((float)i * step  + cosineAngle) % 360.0f - (float)_fov/2.0f;
    //         Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * transform.up;

    //         RaycastHit2D hit = Physics2D.Raycast(pos, dir, _viewRange, layerMask);

    //         Color color = Color.green;
    //         float rayLength = _viewRange;
    //         if (hit.collider != null)
    //         {
    //             // print(
    //             //     "I can see a " + hit.collider.gameObject.name + " at " + hit.distance + " units"
    //             // );
    //             rayLength = hit.distance;
    //             float whosThere = 0;
    //             if (hit.collider.gameObject.name == "Prey")
    //             {
    //                 whosThere = 1.0f;
    //             }
    //             else if (hit.collider.gameObject.name == "Predator")
    //             {
    //                 whosThere = -1.0f;
    //                 color = Color.red;

    //             }

    //             WhoIsThere[i] = whosThere;
    //         }
    //         else
    //         {
    //             //print("I hit nothing");
    //             color = Color.gray;
    //             WhoIsThere[i] = 0.0f;

    //         }
    //         Distances[i] = rayLength < _viewRange ? 1 / rayLength : 0;
    //         if (toggleShowRays)
    //             Debug.DrawRay(pos, dir.normalized * rayLength, color);
    //     }
    // }
}
