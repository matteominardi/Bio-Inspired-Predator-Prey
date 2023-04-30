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
    public GameObject speedPreys;
    public GameObject speedPredators;
    public GameObject dmgPreys;
    public GameObject dmgPredators;
    public GameObject fovPreys;
    public GameObject fovPredators;
    public GameObject numRaysPreys;
    public GameObject numRaysPredators;
    public GameObject viewRangePreys;
    public GameObject viewRangePredators;
    public GameObject mutationRate;
    public GameObject mutationAmount;

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

        speedPreys.GetComponent<TMP_InputField>().text = SceneInitializer.speedPreys.ToString();
        speedPredators.GetComponent<TMP_InputField>().text = SceneInitializer.speedPredators.ToString();
        dmgPreys.GetComponent<TMP_InputField>().text = SceneInitializer.dmgPreys.ToString();
        dmgPredators.GetComponent<TMP_InputField>().text = SceneInitializer.dmgPredators.ToString();
        fovPreys.GetComponent<TMP_InputField>().text = SceneInitializer.fovPreys.ToString();
        fovPredators.GetComponent<TMP_InputField>().text = SceneInitializer.fovPredators.ToString();
        numRaysPreys.GetComponent<TMP_InputField>().text = SceneInitializer.numRaysPreys.ToString();
        numRaysPredators.GetComponent<TMP_InputField>().text = SceneInitializer.numRaysPredators.ToString();
        viewRangePreys.GetComponent<TMP_InputField>().text = SceneInitializer.viewRangePreys.ToString();
        viewRangePredators.GetComponent<TMP_InputField>().text = SceneInitializer.viewRangePredators.ToString();
        mutationRate.GetComponent<TMP_InputField>().text = SceneInitializer.mutationRate.ToString();
        mutationAmount.GetComponent<TMP_InputField>().text = SceneInitializer.mutationAmount.ToString();
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

        SceneInitializer.speedPreys = float.Parse(speedPreys.GetComponent<TMP_InputField>().text);
        SceneInitializer.speedPredators = float.Parse(speedPredators.GetComponent<TMP_InputField>().text);
        SceneInitializer.dmgPreys = int.Parse(dmgPreys.GetComponent<TMP_InputField>().text);
        SceneInitializer.dmgPredators = int.Parse(dmgPredators.GetComponent<TMP_InputField>().text);
        SceneInitializer.fovPreys = int.Parse(fovPreys.GetComponent<TMP_InputField>().text);
        SceneInitializer.fovPredators = int.Parse(fovPredators.GetComponent<TMP_InputField>().text);
        SceneInitializer.numRaysPreys = int.Parse(numRaysPreys.GetComponent<TMP_InputField>().text);
        SceneInitializer.numRaysPredators = int.Parse(numRaysPredators.GetComponent<TMP_InputField>().text);
        SceneInitializer.viewRangePreys = int.Parse(viewRangePreys.GetComponent<TMP_InputField>().text);
        SceneInitializer.viewRangePredators = int.Parse(viewRangePredators.GetComponent<TMP_InputField>().text);
        SceneInitializer.mutationRate = float.Parse(mutationRate.GetComponent<TMP_InputField>().text);
        SceneInitializer.mutationAmount = float.Parse(mutationAmount.GetComponent<TMP_InputField>().text);
    }
}