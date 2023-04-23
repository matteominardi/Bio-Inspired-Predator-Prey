using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CanvasScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     print("mouse down");
        
        //     GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        //     EventSystem m_EventSystem = GetComponent<EventSystem>();

        //     List<RaycastResult> results = new List<RaycastResult>();
        //     PointerEventData ped = new PointerEventData(EventSystem.current);
        //     print("event system: " + EventSystem.current);
        //     print("pointer event: " + ped);
        //     //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector3 mousePos = Input.mousePosition;
        //     //mousePos.z = 85;
        //     print("mouse pos: " + mousePos);
        //     ped.position = new Vector2(mousePos.x, mousePos.y);
        //     print(ped);
        //     gr.Raycast(ped, results);
        //     if (results.Count > 0)
        //     {
        //         print("clicked on UI " + results[0].gameObject.name);
        //         return;
        //     }
        // }
        
    }
}
