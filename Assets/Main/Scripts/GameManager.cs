using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Station> stations;

    [HideInInspector] public GameObject player;

    private void Awake() {
        if(instance != this)
            instance = this;
        else
            Destroy(this);
    }

    private void Update() {
        if(stations.Count == 0)
            return;

        if(AllStationsCaptured()) {
            print("GAME OVER");
        }
    }

    private bool AllStationsCaptured() {
        // foreach(Station station in stations) {
        //     if(!station.captured)
        //         return false;
        // }
        // return true;
        return false;
    }
}
