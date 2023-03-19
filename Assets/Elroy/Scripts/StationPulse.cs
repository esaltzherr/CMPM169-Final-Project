using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPulse : MonoBehaviour
{
    // Start is called before the first frame update
    Station station;
    UnityEngine.Rendering.Universal.Light2D lights;
    void Start()
    {
        station = GetComponentInParent<Station>();
        lights = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameOver){
            StopAllCoroutines();
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D obj){
        if (obj.gameObject.name == "Player"){
            lights.enabled = true;
        }
        
    }
    
    private void OnTriggerExit2D(Collider2D obj){
        if (obj.gameObject.name == "Player"){
            lights.enabled = false;
        }
    }
}
