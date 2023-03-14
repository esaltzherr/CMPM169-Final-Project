using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector] public GameObject player;

    private void Awake() {
        if(instance != this)
            instance = this;
        else
            Destroy(this);
    }
}
