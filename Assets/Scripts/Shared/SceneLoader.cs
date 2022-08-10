using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;

    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    private IEnumerator InitializeSceneLoading()
    {
        // First load the Loading Scene
        //yield return SceneManager.LoadSceneAsync("Scene_Loading");
        yield return null;

        // Load the actual scene
        StartCoroutine(LoadActualScene());
    }

    private IEnumerator LoadActualScene()
    {
        var asyncSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
            //Debug.Log(asyncSceneLoading.progress);
            if (asyncSceneLoading.progress >= 0.9f)
            {
                asyncSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void LoadGestureCustomization()
    {
        LoadScene("Gesture Cutomization");
    }

    public void LoadParticipantTesting()
    {
        LoadScene("Participiant_Testing_Scene");
    }

    public void LoadSelect()
    {
        LoadScene("SceneSelect");
    }
}