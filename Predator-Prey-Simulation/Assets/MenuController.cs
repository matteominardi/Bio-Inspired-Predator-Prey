using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MenuController : MonoBehaviour
{
    //public static SceneInitializer sceneInitializer;
    public GameObject numPreyInput;
    public GameObject numPredatorInput;
    public GameObject maxPreyInput;
    public GameObject maxPredatorInput;
    public GameObject loadPretrainedInput;
    public GameObject mapSizeInput;
    // private StartGameButton startGameButton;
    // private LoadingScreenController loadingScreen;

    void Start()
    {
        // Debug.Log("MenuController.Start()");
        // print("numPreyInput " + numPreyInput);
        // TMPro.TMP_InputField inputNumPrey = numPreyInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        // print("inputNumPrey: " + inputNumPrey);
        numPreyInput.gameObject.GetComponent<TMP_InputField>().text = SceneInitializer.NUMPREY.ToString();
        numPredatorInput.GetComponent<TMP_InputField>().text = SceneInitializer.NUMPREDATOR.ToString();
        maxPreyInput.GetComponent<TMP_InputField>().text = SceneInitializer.MAXPREYALLOWED.ToString();
        maxPredatorInput.GetComponent<TMP_InputField>().text = SceneInitializer.MAXPREDATORALLOWED.ToString();
        loadPretrainedInput.GetComponent<Toggle>().isOn = SceneInitializer.loadPretrained;
        mapSizeInput.GetComponent<TMP_InputField>().text = SceneInitializer.mapSize.ToString();
        //loadingScreen = new LoadingScreenController();
        //startGameButton = new StartGameButton(this, loadingScreen);
    }

    public void ConfirmChanges()
    {
        SceneInitializer.NUMPREY = int.Parse(numPreyInput.GetComponent<TMP_InputField>().text);
        SceneInitializer.NUMPREDATOR = int.Parse(numPredatorInput.GetComponent<TMP_InputField>().text);
        SceneInitializer.MAXPREYALLOWED = int.Parse(maxPreyInput.GetComponent<TMP_InputField>().text);
        SceneInitializer.MAXPREDATORALLOWED = int.Parse(maxPredatorInput.GetComponent<TMP_InputField>().text);
        SceneInitializer.loadPretrained = loadPretrainedInput.GetComponent<Toggle>().isOn;
        SceneInitializer.mapSize = int.Parse(mapSizeInput.GetComponent<TMP_InputField>().text);
    }
}