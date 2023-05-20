using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 20.0f;
    public float dragSpeed = 15f;
    public GameObject Canvas;
    public GameObject target;
    private int _mapSize;
    private float _maxZoom;
    private float _minZoom;

    public Entity targetEntity;
    private EntityManager entityManager;
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        _mapSize = SceneInitializerECS.mapSize * 10;
        _maxZoom = -_mapSize;
        _minZoom = -10.46f;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //offset = transform.position - entityManager.GetComponentData<Translation>(targetEntity).Value;
    }

    // Update is called once per frame
    void Update()
    {
        // float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        // if (scrollWheel != 0)
        // {
        //     Vector3 oldPos = transform.position;
        //     float clampedZ = Mathf.Clamp(oldPos.z + scrollWheel * zoomSpeed, -_mapSize, -10.46f);
        //     oldPos.z = clampedZ;
        //     transform.position = oldPos;
        //     //Canvas.GetComponent<Canvas>().planeDistance += scrollWheel * zoomSpeed;
        // }
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollWheel != 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distanceToZoomPoint = Vector3.Dot(transform.position - ray.origin, Camera.main.transform.forward);
            Vector3 zoomPoint = ray.GetPoint(distanceToZoomPoint);

            float clampedZ = Mathf.Clamp(transform.position.z + scrollWheel * zoomSpeed, _maxZoom, _minZoom);
            Vector3 direction = (zoomPoint - transform.position).normalized;
            Vector3 newPos = transform.position + direction * scrollWheel * zoomSpeed;

            if (newPos.z >= _maxZoom && newPos.z <= _minZoom)
            {
                transform.position = newPos;
            }
        }

        // float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // if (scrollWheel != 0)
        // {
        //     Vector3 oldPos = transform.position;
        //     float zoomAmount = scrollWheel * zoomSpeed;

        //     // Calculate the zoom pivot point in world coordinates based on the mouse position
        //     Vector3 zoomPivot = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector3 pivotOffset = zoomPivot - oldPos;

        //     // Apply zoom
        //     oldPos += pivotOffset * zoomAmount;
        //     oldPos.z = Mathf.Clamp(oldPos.z, -_mapSize, -10.46f);
        //     transform.position = oldPos;
        // }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Reset();
        }

        if (Input.GetMouseButton(1))
        {
            float dragX = -Input.GetAxis("Mouse X") * dragSpeed;
            float dragY = -Input.GetAxis("Mouse Y") * dragSpeed;
            transform.position += new Vector3(dragX, dragY, 0);
        }

        // if (target != null)
        // {
        //     Vector3 newPos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        //     transform.position = Vector3.MoveTowards(transform.position, newPos, 3);
        // }
        if (targetEntity != Entity.Null && entityManager.Exists(targetEntity))
        {
            float3 targetPos = entityManager.GetComponentData<Translation>(targetEntity).Value;
            Vector3 newPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, 0.125f);
        }
    }

    public void Reset(bool detachOnly = false)
    {
        targetEntity = Entity.Null;
        if (!detachOnly)
            transform.position = new Vector3(0, 0, -50f);
    }
}
