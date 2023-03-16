using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Station> stations;

    [HideInInspector] public GameObject player;
    private PlayerController pController;

    private void Awake() {
        if(instance != this)
            instance = this;
        else
            Destroy(this);
    }

    private void Update() {
        if(player&& !pController)
            pController = player.GetComponent<PlayerController>();

        if(AllStationsCaptured()) {
            print("WIN");
        }
        else if(pController && pController.hp <= 0) {
            print("LOSE");
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
}
