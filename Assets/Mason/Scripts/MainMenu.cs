using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<string> scenes;
    [SerializeField] private List<GameObject> menuGameObjects;

    public void PlayButton() {
        foreach(GameObject obj in menuGameObjects)
            obj.SetActive(false);
        StartCoroutine("LoadScenes");
    }

    private IEnumerator LoadScenes() {
        foreach(string scene in scenes) {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }
        yield return new WaitUntil(() => DoneLoading());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes[0]));
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    private bool DoneLoading() {
        foreach(string scene in scenes) {
            if(!SceneManager.GetSceneByName(scene).isLoaded)
                return false;
        }
        return true;
    }
}
