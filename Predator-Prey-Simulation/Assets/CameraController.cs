using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2.0f;
    public float dragSpeed = 1.5f;
    public GameObject Canvas;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scrollWheel * zoomSpeed;
        //Canvas.GetComponent<Canvas>().planeDistance += scrollWheel * zoomSpeed;


        if (Input.GetMouseButton(0))
        {
            float dragX = -Input.GetAxis("Mouse X") * dragSpeed;
            float dragY = -Input.GetAxis("Mouse Y") * dragSpeed;
            transform.position += new Vector3(dragX, dragY, 0);
        }
    }
}
