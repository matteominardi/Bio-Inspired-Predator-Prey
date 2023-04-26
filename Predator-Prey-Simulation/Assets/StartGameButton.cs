using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGameButton : MonoBehaviour
{
    // MenuController menuController;
    public string gameSceneName = "GameScene";
    // public LoadingScreenController loadingScreen; // a reference to a loading screen object
    public Button yourButton;
    public GameObject menuController;
    public GameObject loadingScreen;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    // public StartGameButton(MenuController menuController, LoadingScreenController loadingScreen)
    // {
    //     this.menuController = menuController;
    //     this.loadingScreen = loadingScreen;
    // }

    public void OnClick()
    {
        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        // Show loading screen
        loadingScreen.gameObject.SetActive(true);

        // Load game scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single);

        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(progress);
            yield return null;
        }

        // Hide loading screen
        loadingScreen.gameObject.SetActive(false);
        SceneManager.UnloadSceneAsync(gameObject.scene);
        //SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        menuController.GetComponent<MenuController>().ConfirmChanges();

    }
}