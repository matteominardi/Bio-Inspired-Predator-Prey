using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2.0f;
    public float dragSpeed = 1.5f;
    public GameObject Canvas;
    public GameObject target;
    private int _mapSize;


    // Start is called before the first frame update
    void Start()
    {
        _mapSize = SceneInitializer.mapSize * 10;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            Vector3 oldPos = transform.position;
            float clampedZ = Mathf.Clamp(oldPos.z + scrollWheel * zoomSpeed, -_mapSize, -10.46f);
            oldPos.z = clampedZ;
            transform.position = oldPos;
            //Canvas.GetComponent<Canvas>().planeDistance += scrollWheel * zoomSpeed;
        }

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

        if (target != null)
        {
            Vector3 newPos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, 3);
        }
    }

    public void Reset(bool detachOnly = false)
    {
        target = null;
        if (!detachOnly)
            transform.position = new Vector3(0, 0, -50f);
    }
}
