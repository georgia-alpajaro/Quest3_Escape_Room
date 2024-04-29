using Fusion;
using Fusion.Addons.ConnectionManagerAddon;
using Photon.Voice;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    private string sceneToLoad;
    public GameObject loadingScreen;
    public TextMeshProUGUI loadingPercent;
    public NetworkRunner runner;

    private void Start()
    {
        loadingScreen.SetActive(false);
        loadingPercent = loadingScreen.transform.Find("Panel/LoadingPercent").GetComponent<TextMeshProUGUI>();
        runner = ConnectionManager.Instance.GetComponent<NetworkRunner>();
    }

    public void SwitchToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(LoadScene());
        //show loading screen
        loadingScreen.SetActive(true);
    }

    private IEnumerator LoadScene()
    {
        /*
        NetworkSceneAsyncOp asyncOp = runner.LoadScene(sceneToLoad);
        while (!asyncOp.IsDone)
        {
            yield return null;
        }
        runner.UnloadScene(SceneManager.GetActiveScene().name);
        */
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            loadingPercent.text = (asyncLoad.progress * 100) + "%";

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        loadingScreen.SetActive(false);
        
    }
}
