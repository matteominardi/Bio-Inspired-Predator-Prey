using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    Camera mainCamera = Camera.main;
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 300))
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.tag == "SelectableObject")
            {
                if (hit.collider.gameObject.name == "Predator")
                {
                    var selectedObject = hit.collider.gameObject.GetComponent<Predator>();
                }
                else if (hit.collider.gameObject.name == "Prey") {
                    
                }
                    var selectedObject = hit.collider.gameObject.GetComponent<Prey>();



            }

            Debug.DrawRay(ray.origin, ray.direction * 300, Color.blue);
        }
    }
}
