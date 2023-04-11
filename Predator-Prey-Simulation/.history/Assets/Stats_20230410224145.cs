using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    Camera mainCamera;
    ISelectable selectedObject;
    // Start is called before the first frame update
    void Start() 
    { 
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin, ray.direction * 300, Color.red);
        // print("ray " + ray);
        // RaycastHit2D hit = new RaycastHit2D();

        // print(Physics.Raycast(ray, out hit));

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.tag == "SelectableObject")
            {
                print("clicked on " + hit.collider.gameObject.name);
                ISelectable obj;
                if (hit.collider.gameObject.name == "Predator")
                {
                    obj = hit.collider.gameObject.GetComponent<Predator>();
                    selectedObject = obj;
                }
                else
                {
                    obj = hit.collider.gameObject.GetComponent<Prey>();
                }
                string health = obj.Lifepoints.ToString();
                string fitness = obj.Fitness.ToString();
                string energy = obj.Energy.ToString();
                string speed = obj.Speed.ToString();
                string alive = obj.Alive.ToString();
                
                //print(gameObject.transform.Find("lblHealth").transform.Find("txtHealth").GetComponent<TMPro.TextMeshProUGUI>());//.transform.Find("txtHealth").GetComponents<MonoBehaviour>());
                gameObject.transform.Find("lblHealth").transform.Find("txtHealth").GetComponent<TMPro.TextMeshProUGUI>().text = health;
                //gameObject.transform.Find("lblFitness").transform.Find("txtFitness").GetComponent<TMPro.TextMeshProUGUI>().text = fitness;
                gameObject.transform.Find("lblEnergy").transform.Find("txtEnergy").GetComponent<TMPro.TextMeshProUGUI>().text = energy;
                gameObject.transform.Find("lblSpeed").transform.Find("txtSpeed").GetComponent<TMPro.TextMeshProUGUI>().text = speed;
                gameObject.transform.Find("lblAlive").transform.Find("txtAlive").GetComponent<TMPro.TextMeshProUGUI>().text = alive;

                //GetComponent<TMPro.TextMeshProUGUI>().text = health;


            }

            //Debug.DrawRay(ray.origin, ray.direction * 300, Color.blue);
        }
    }
}
