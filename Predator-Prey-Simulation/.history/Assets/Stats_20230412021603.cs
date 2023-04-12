using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    Camera mainCamera;
    ISelectable selectedObject;
    TMPro.TextMeshProUGUI refTxtHealth;
    TMPro.TextMeshProUGUI refTxtFitness;
    TMPro.TextMeshProUGUI refTxtEnergy;
    TMPro.TextMeshProUGUI refTxtSpeed;
    TMPro.TextMeshProUGUI refTxtAlive;
    TMPro.TextMeshProUGUI refTxtGeneration;

    bool visible = false;
    // Start is called before the first frame update
    void Start() 
    {
        mainCamera = Camera.main;
        refTxtHealth = gameObject.transform.Find("lblHealth").transform.Find("txtHealth").GetComponent<TMPro.TextMeshProUGUI>();
        refTxtFitness = gameObject.transform.Find("lblFitness").transform.Find("txtFitness").GetComponent<TMPro.TextMeshProUGUI>();
        refTxtEnergy = gameObject.transform.Find("lblEnergy").transform.Find("txtEnergy").GetComponent<TMPro.TextMeshProUGUI>();
        refTxtSpeed = gameObject.transform.Find("lblSpeed").transform.Find("txtSpeed").GetComponent<TMPro.TextMeshProUGUI>();
        refTxtAlive = gameObject.transform.Find("lblAlive").transform.Find("txtAlive").GetComponent<TMPro.TextMeshProUGUI>();
        refTxtGeneration = gameObject.transform.Find("lblGeneration").transform.Find("txtGeneration").GetComponent<TMPro.TextMeshProUGUI>();
        transform.parent.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.tag == "SelectableObject")
            {
                //print("clicked on " + hit.collider.gameObject.name);
                ISelectable obj;
                if (hit.collider.gameObject.name == "Predator")
                {
                    obj = hit.collider.gameObject.GetComponent<Predator>();
                }
                else if (hit.collider.gameObject.name == "Prey")
                {
                    obj = hit.collider.gameObject.GetComponent<Prey>();
                }
                else
                    return;

                if (visible == false)
                {
                    transform.parent.GetComponent<CanvasGroup>().alpha = 1;
                    visible = true;
                }
                selectedObject = obj;
                string health = obj.Lifepoints.ToString();
                string fitness = ((int)obj.Fitness).ToString();
                string energy = obj.Energy.ToString("n2");
                string speed = obj.Speed.ToString();
                string alive = obj.Alive.ToString();
                
                refTxtHealth.text = health;
                refTxtFitness.text = fitness;
                refTxtEnergy.text = energy;
                refTxtSpeed.text = speed;
                refTxtAlive.text = alive;
                refTxtGeneration.text = generati

                mainCamera.GetComponent<MyCamera>().target = hit.collider.gameObject;
                return;

                //GetComponent<TMPro.TextMeshProUGUI>().text = health;
            }

            //Debug.DrawRay(ray.origin, ray.direction * 300, Color.blue);
        }
        if (selectedObject != null)
        {
            string health = selectedObject.Lifepoints.ToString();
            string fitness = ((int)selectedObject.Fitness).ToString();
            string energy = selectedObject.Energy.ToString("n2");
            string speed = selectedObject.Speed.ToString();
            string alive = selectedObject.Alive.ToString();
            
            //print(gameObject.transform.Find("lblHealth").transform.Find("txtHealth").GetComponent<TMPro.TextMeshProUGUI>());//.transform.Find("txtHealth").GetComponents<MonoBehaviour>());
            refTxtHealth.text = health;
            refTxtFitness.text = fitness;
            refTxtEnergy.text = energy;
            refTxtSpeed.text = speed;
            refTxtAlive.text = alive;
        }
        if (Input.GetMouseButtonDown(0) && selectedObject != null) {
            selectedObject = null;
            refTxtHealth.text = "";
            refTxtFitness.text = "";
            refTxtEnergy.text = "";
            refTxtSpeed.text = "";
            refTxtAlive.text = "";
            mainCamera.GetComponent<MyCamera>().Reset();
        }
    }
}
