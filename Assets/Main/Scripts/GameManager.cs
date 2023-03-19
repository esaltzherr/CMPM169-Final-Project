using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [HideInInspector] public GameObject player;
    public bool gameOver { get; private set; }
    public List<Station> stations;
    private PlayerController pController;

    private void Awake() {
        if(instance != this)
            instance = this;
        else
            Destroy(this);

        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    private void Update() {
        if(gameOver) return;

        if(player && !pController)
            pController = player.GetComponent<PlayerController>();

        if(AllStationsCaptured()) {
            MenuScreen(winScreen);
            gameOver = true;
        }
        else if(pController && pController.hp <= 0) {
            MenuScreen(loseScreen);
            gameOver = true;
        }
    }

    private bool AllStationsCaptured() {
        if(stations.Count == 0) return false;
        
        foreach(Station station in stations) {
            if(!station.captured)
                return false;
        }
        return true;
    }

    private void MenuScreen(GameObject screen) {
        screen.SetActive(true);
        screen.GetComponent<Animator>().SetTrigger("FadeIn");
    }

    public void LoadMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
