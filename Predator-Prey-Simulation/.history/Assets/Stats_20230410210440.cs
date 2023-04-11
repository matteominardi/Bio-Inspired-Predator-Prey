using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = thiscamera.ScreenPointToRay(Input.mousePosition);

    RaycastHit hit = new RaycastHit();

    if (Physics.Raycast(ray, out hit, 300))
    {
        if (Input.GetMouseButtonDown(0) && hit.transform.tag == "SelectableObject")
        {
          selectedObject = hit.gameObject;
        }

        Debug.DrawRay(ray.origin, ray.direction * 300, Color.blue);
    }
    }
}
