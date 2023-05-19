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
        //btn.onClick.AddListener(OnClick);
    }

    // public StartGameButton(MenuController menuController, LoadingScreenController loadingScreen)
    // {
    //     this.menuController = menuController;
    //     this.loadingScreen = loadingScreen;
    // }

    public void OnClick()
    {
        //print("StartGameButton.OnClick()");
        menuController.GetComponent<MenuController>().ConfirmChanges();
        //print("StartGameButton after click");

        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        // Show loading screen
        loadingScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(0.2f);

        yield return new WaitForSeconds(0.5f);

        loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(0.5f);

        yield return new WaitForSeconds(1f);

        loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(0.8f);

        yield return new WaitForSeconds(0.5f);
        // Load game scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);


        // float fakeProgress = 0f;
        // // Wait until the scene is loaded
        // while (fakeProgress <= 100f)
        // {
        //     fakeProgress += 1f;
        //     loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(fakeProgress/100f);
        //     yield return new WaitForSeconds(10f);
        // }

        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingScreen.GetComponent<LoadingScreenController>().UpdateProgress(progress);
            
            yield return new WaitForSecondsRealtime(1f);
        }

        // Hide loading screen
        loadingScreen.gameObject.SetActive(false);
        SceneManager.UnloadSceneAsync(gameObject.scene);
        //SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        

    }
}