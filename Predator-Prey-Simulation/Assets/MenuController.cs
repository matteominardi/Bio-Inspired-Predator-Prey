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
    public GameObject deviationAmount;
    public GameObject timerReproductionPreys;
    public GameObject energyGainPreys;
    public GameObject energyLossPredators;
    public GameObject reproductionGainPredators;

    // private StartGameButton startGameButton;
    // private LoadingScreenController loadingScreen;

    void Start()
    {
        // Debug.Log("MenuController.Start()");
        // print("numPreyInput " + numPreyInput);
        // TMPro.TMP_InputField inputNumPrey = numPreyInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        // print("inputNumPrey: " + inputNumPrey);
        numPreyInput.gameObject.GetComponent<TMP_InputField>().text = SceneInitializerECS.NUMPREY.ToString();
        numPredatorInput.GetComponent<TMP_InputField>().text = SceneInitializerECS.NUMPREDATOR.ToString();
        maxPreyInput.GetComponent<TMP_InputField>().text = SceneInitializerECS.MAXPREYALLOWED.ToString();
        maxPredatorInput.GetComponent<TMP_InputField>().text = SceneInitializerECS.MAXPREDATORALLOWED.ToString();
        loadPretrainedInput.GetComponent<Toggle>().isOn = SceneInitializerECS.loadPretrained;
        mapSizeInput.GetComponent<TMP_InputField>().text = SceneInitializerECS.mapSize.ToString();

        speedPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.speedPreys.ToString();
        speedPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.speedPredators.ToString();
        dmgPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.dmgPreys.ToString();
        dmgPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.dmgPredators.ToString();
        fovPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.fovPreys.ToString();
        fovPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.fovPredators.ToString();
        numRaysPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.numRaysPreys.ToString();
        numRaysPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.numRaysPredators.ToString();
        viewRangePreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.viewRangePreys.ToString();
        viewRangePredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.viewRangePredators.ToString();
        mutationRate.GetComponent<TMP_InputField>().text = SceneInitializerECS.mutationRate.ToString();
        mutationAmount.GetComponent<TMP_InputField>().text = SceneInitializerECS.mutationAmount.ToString();
        deviationAmount.GetComponent<TMP_InputField>().text = SceneInitializerECS.deviationAmount.ToString();
        timerReproductionPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.timerReproductionPreys.ToString();
        energyGainPreys.GetComponent<TMP_InputField>().text = SceneInitializerECS.energyGainPreys.ToString();
        energyLossPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.energyLossPredators.ToString();
        reproductionGainPredators.GetComponent<TMP_InputField>().text = SceneInitializerECS.reprodGainWhenEatPredators.ToString();
        //loadingScreen = new LoadingScreenController();
        //startGameButton = new StartGameButton(this, loadingScreen);
    }

    public void ConfirmChanges()
    {
        SceneInitializerECS.NUMPREY = int.Parse(numPreyInput.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.NUMPREDATOR = int.Parse(numPredatorInput.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.MAXPREYALLOWED = int.Parse(maxPreyInput.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.MAXPREDATORALLOWED = int.Parse(maxPredatorInput.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.loadPretrained = loadPretrainedInput.GetComponent<Toggle>().isOn;
        SceneInitializerECS.mapSize = int.Parse(mapSizeInput.GetComponent<TMP_InputField>().text);

        SceneInitializerECS.speedPreys = float.Parse(speedPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.speedPredators = float.Parse(speedPredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.dmgPreys = int.Parse(dmgPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.dmgPredators = int.Parse(dmgPredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.fovPreys = int.Parse(fovPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.fovPredators = int.Parse(fovPredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.numRaysPreys = int.Parse(numRaysPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.numRaysPredators = int.Parse(numRaysPredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.viewRangePreys = int.Parse(viewRangePreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.viewRangePredators = int.Parse(viewRangePredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.mutationRate = float.Parse(mutationRate.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.mutationAmount = float.Parse(mutationAmount.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.deviationAmount = float.Parse(deviationAmount.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.timerReproductionPreys = int.Parse(timerReproductionPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.energyGainPreys = float.Parse(energyGainPreys.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.energyLossPredators = float.Parse(energyLossPredators.GetComponent<TMP_InputField>().text);
        SceneInitializerECS.reprodGainWhenEatPredators = float.Parse(reproductionGainPredators.GetComponent<TMP_InputField>().text);
    }
}