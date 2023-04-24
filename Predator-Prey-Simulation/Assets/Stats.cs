using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vectrosity;

public class Stats : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] public GameObject CanvasParent;
    public GameObject neuronPrefab;
    ISelectable selectedObject;
    TMPro.TextMeshProUGUI refTxtHealth;
    TMPro.TextMeshProUGUI refTxtFitness;
    TMPro.TextMeshProUGUI refTxtEnergy;
    TMPro.TextMeshProUGUI refTxtSpeed;
    TMPro.TextMeshProUGUI refTxtAlive;
    TMPro.TextMeshProUGUI refTxtGeneration;

    Toggle toggleButton;
    [SerializeField] private LayerMask layerMask;
    GraphicRaycaster gr;

    GameObject panelBrain;
    RectTransform panelBrainRect;
    GameObject[][] Neurons;
    VectorLine[][][] NeuronsLinks;

    bool firstDraw = true;


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
        toggleButton = gameObject.transform.Find("Toggle").GetComponent<Toggle>();
        layerMask = ~((1<<5));

        gr = GetComponent<GraphicRaycaster>();
        panelBrain = gameObject.transform.Find("PanelBrain").gameObject;
        panelBrainRect = panelBrain.GetComponent<RectTransform>();

        //VectorManager. = Camera.main;
        float panelWidth = panelBrain.GetComponent<RectTransform>().rect.width - 30;
        //print("panelWidth " + panelWidth);
        float panelHeight = panelBrain.GetComponent<RectTransform>().rect.height - 30;
        //float[] neuronsActivationsDistances = obj.gameObject.GetComponent<Raycast>().Distances;
        //float[] neuronsActivationsWhoIsThere = obj.gameObject.GetComponent<Raycast>().WhoIsThere;
        





        // ------------------ NEURAL NETWORK ------------------
        //VectorLine.SetCanvasCamera(mainCamera);
        //VectorLine.SetCanvas(GameObject.Find("Canvas"));
        VectorLine.canvas.sortingOrder = 4;


        // ------------------ NEURONS -----------------------
        int[] brainStructure = SceneInitializer.BrainStructure();
        Neurons = new GameObject[brainStructure.Length][];
        for (int i = 0; i < brainStructure.Length; i++)
        {
            Neurons[i] = new GameObject[brainStructure[i]];
        }

        // ------------------ NEURONS LINKS ------------------
        NeuronsLinks = new VectorLine[brainStructure.Length-1][][];
        for (int i = 0; i < brainStructure.Length-1; i++)
        {
            NeuronsLinks[i] = new VectorLine[brainStructure[i]][];
            for (int j = 0; j < brainStructure[i]; j++)
            {
                NeuronsLinks[i][j]= new VectorLine[brainStructure[i+1]];
            }
        }
        // loop over layers
        for (int i = 0; i < brainStructure.Length; i++)
        {
            // loop over neurons
            for (int j = 0; j < brainStructure[i]; j++)
            {
                // draw neuron
                GameObject neuron = Instantiate<GameObject>(neuronPrefab, panelBrain.transform);
                Vector2 pos = new Vector2((float)i/(float)(brainStructure.Length - 1)* panelWidth+15f, -(float)j / (float)(brainStructure[i]-1) * panelHeight-15f);
                neuron.GetComponent<RectTransform>().localPosition = pos;
                neuron.SetActive(false);
                Neurons[i][j] = neuron;
                Vector2 posLine = panelBrainRect.TransformPoint(pos);
                // set activation according to raycast input
                
                // loop over next layer to draw lines
                if (i+1 < brainStructure.Length)
                {
                    for (int k = 0; k < brainStructure[i+1]; k++)
                    {
                        Vector2 nextPos = new Vector2((float)(i+1)/(float)(brainStructure.Length - 1)* panelWidth+15f, -(float)k / (float)(brainStructure[i+1]-1) * panelHeight-15f);
                        Vector2 nextPosLine = panelBrainRect.TransformPoint(nextPos);
                        float widthLine = panelBrainRect.TransformPoint(new Vector2(1f, 1f))[0];
                        NeuronsLinks[i][j][k] = new VectorLine("NeuronLink-"+i+"/"+j+"/"+k, new List<Vector2> { posLine, nextPosLine }, 0.06f, LineType.Discrete);
                        //NeuronsLinks[i][j][k].color = Color.white;
                        NeuronsLinks[i][j][k].active = false;
                        NeuronsLinks[i][j][k].SetCanvas(CanvasParent);
                        //NeuronsLinks[i][j][k].Draw();
                        //VectorLine.SetLine(Color.white, panelBrainRect.TransformPoint(pos), panelBrainRect.TransformPoint(nextPos));
                    }
                }

            }
            //print("x" + (float)i / (float)brainModel.Length * panelWidth + 15f + " y " + (float)i / (float)brainModel.Length * panelHeight + 15f);
            //neuron.GetComponent<RectTransform>().anchoredPosition = new Vector2(brainModel[i] % 10 * panelWidth / 10, brainModel[i] / 10 * panelHeight / 10);
            //neuron.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth / 10, panelHeight / 10);
            //neuron.GetComponent<Image>().color = Color.white;
        }

        transform.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
            {
                if ((hit.transform.tag == "SelectableObject" || hit.transform.tag == "Predator" || hit.transform.tag == "Prey"))
                {
                    //print("clicked on " + hit.collider.gameObject.name);
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
                        transform.GetComponent<CanvasGroup>().alpha = 1;
                        visible = true;
                    }
                    selectedObject = obj;
                    string health = obj.Lifepoints.ToString();
                    string fitness = ((int)obj.Fitness).ToString();
                    string energy = obj.Energy.ToString("n2");
                    string speed = obj.Speed.ToString();
                    string alive = obj.Alive.ToString();
                    string generation = obj.Generation.ToString();
                    
                    refTxtHealth.text = health;
                    refTxtFitness.text = fitness;
                    refTxtEnergy.text = energy;
                    refTxtSpeed.text = speed;
                    refTxtAlive.text = alive;
                    refTxtGeneration.text = generation;
                    toggleButton.isOn = obj.Raycast.toggleShowRays;

                    mainCamera.GetComponent<MyCamera>().target = hit.collider.gameObject;

                    ShowBrain(obj);
                    return;

                    //GetComponent<TMPro.TextMeshProUGUI>().text = health;
                }
                else {
                    print("clicked on something with another tag");
                    return;
                }
                
                

                //Debug.DrawRay(ray.origin, ray.direction * 300, Color.blue);
            }
            else 
            {
                List<RaycastResult> results = new List<RaycastResult>();
                PointerEventData ped = new PointerEventData(EventSystem.current);
                ped.position = Input.mousePosition;
                gr.Raycast(ped, results);
                if (results.Count > 0)
                {
                    //print("INSIDE clicked on UI " + results[0].gameObject.name);
                    if (results[0].gameObject.name == "Checkmark" && selectedObject != null)
                        selectedObject.Raycast.toggleShowRays = !selectedObject.Raycast.toggleShowRays;
                }
                else if (selectedObject != null) {
                    selectedObject = null;
                    refTxtHealth.text = "";
                    refTxtFitness.text = "";
                    refTxtEnergy.text = "";
                    refTxtSpeed.text = "";
                    refTxtAlive.text = "";
                    refTxtGeneration.text = "";
                    toggleButton.isOn = false;
                    mainCamera.GetComponent<MyCamera>().Reset();
                    visible = false;
                    transform.GetComponent<CanvasGroup>().alpha = 0;
                    HideBrain(selectedObject);
                }
                return;
            }

        }
        
        if (selectedObject != null)
        {
            string health = selectedObject.Lifepoints.ToString();
            string fitness = ((int)selectedObject.Fitness).ToString();
            string energy = selectedObject.Energy.ToString("n2");
            string speed = selectedObject.Speed.ToString();
            string alive = selectedObject.Alive.ToString();
            string generation = selectedObject.Generation.ToString();
            
            //print(gameObject.transform.Find("lblHealth").transform.Find("txtHealth").GetComponent<TMPro.TextMeshProUGUI>());//.transform.Find("txtHealth").GetComponents<MonoBehaviour>());
            refTxtHealth.text = health;
            refTxtFitness.text = fitness;
            refTxtEnergy.text = energy;
            refTxtSpeed.text = speed;
            refTxtAlive.text = alive;
            ShowBrain(selectedObject);
        }
    }

    void ShowBrain(ISelectable obj)
    {
        if (obj.Alive == false)
            return;
        int[] brainModel = obj.BrainModel;
        float panelWidth = panelBrain.GetComponent<RectTransform>().rect.width - 30;
        //print("panelWidth " + panelWidth);
        float panelHeight = panelBrain.GetComponent<RectTransform>().rect.height - 30;
        float[] neuronsActivationsDistances = obj.gameObject.GetComponent<Raycast>().Distances;
        float[] neuronsActivationsWhoIsThere = obj.gameObject.GetComponent<Raycast>().WhoIsThere;
        //print("neuronsActivations " + neuronsActivations.Length);

        // loop over layers
        for (int i = 0; i < brainModel.Length; i++)
        {
            // loop over neurons
            for (int j = 0; j < brainModel[i]; j++)
            {
                GameObject neuron = Neurons[i][j];
                neuron.SetActive(true);
                // set activation according to raycast input
                if (i == 0)
                {
                    if (j % 2 == 0)
                    {
                        float activation = neuronsActivationsDistances[j/2] != 0f ? 1f/neuronsActivationsDistances[j/2] : 0f;
                        float ViewRange = obj.gameObject.GetComponent<Raycast>().ViewRange;
                        float activationNormalized = (activation / ViewRange);
                        //print("activationNormalized " + activationNormalized + " activation " + activation + " ViewRange " + ViewRange + " neuronsActivationsDistances[j/2] " + neuronsActivationsDistances[j/2]);
                        if (activation != 0f)
                        {

                            neuron.transform.Find("Activation").GetComponent<RectTransform>().localScale = new Vector3(activationNormalized, activationNormalized, 1);
                        }
                        else 
                            neuron.transform.Find("Activation").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                    }
                    else 
                    {
                        //print("j "+ j/2);
                        float activation = Mathf.Abs(neuronsActivationsWhoIsThere[j/2]) == 1f ? 1f : 0f;
                        neuron.transform.Find("Activation").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        if (activation == 1f)
                            neuron.transform.Find("Activation").GetComponent<SpriteRenderer>().color = neuronsActivationsWhoIsThere[j/2] == 1 ? Color.green : Color.red;
                        else 
                            neuron.transform.Find("Activation").GetComponent<SpriteRenderer>().color = Color.grey;
                    }
                }
                if (firstDraw && i+1 < brainModel.Length)
                {
                    for (int k = 0; k < brainModel[i + 1]; k++)
                    {
                        float value = obj.Brain[i,j,k]; // your value within the range from -inf to +inf
                        value = Mathf.Clamp(value, -1, 1);
                        // //float zeroToOne = Mathf.InverseLerp(float.NegativeInfinity, float.PositiveInfinity, value);
                        float zeroToOne = Mathf.InverseLerp(-1, 1, value);
                        Color color = Color.Lerp(Color.black, Color.white, zeroToOne);
                        VectorLine neuronLine = NeuronsLinks[i][j][k];
                        neuronLine.active = true;
                        neuronLine.SetColor(color);
                        //print("Line " + i + " " + j + " " + k + " " + neuronLine.GetColor(0));
                        neuronLine.Draw();
                    }
                }

            }
        }
        if (firstDraw)
            firstDraw = false;

    }

    void HideBrain(ISelectable obj)
    {
        for (int i = 0; i < Neurons.Length; i++)
        {
            for (int j = 0; j < Neurons[i].Length; j++)
            {
                GameObject neuron = Neurons[i][j];
                neuron.SetActive(false);
                neuron.transform.Find("Activation").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                //neuron.transform.Find("Activation").GetComponent<SpriteRenderer>().color = Color.white;
                if (i+1 < Neurons.Length)
                {
                    for (int k = 0; k < Neurons[i+1].Length; k++)
                    {
                        VectorLine neuronLine = NeuronsLinks[i][j][k];
                        neuronLine.active = false;
                    }
                }
            }
        }
        firstDraw = true;
    }
}
