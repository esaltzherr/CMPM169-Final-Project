using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] int radius;
    [SerializeField] int ClaimRadius;
    [SerializeField] int PulseSpeed;
    [SerializeField] int PulseBetween;
    [SerializeField] int CapturePings;
    [SerializeField] int CurrentPings;
    [SerializeField] int DeChargeRate;
    [SerializeField] Color NewColor;

    bool within = false;

    SpriteRenderer child;
    
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).GetComponent<SpriteRenderer>();
        print(child);
    }

    // Update is called once per frame
    void Update()
    {
        if(within)
        {
            Pulse();
            // if(GameManager.instance.ping){
            //     CurrentPings++;
            //     if(CurrentPings >= CapturePings){
            //         //Change Color to green to show captured.
            //     }
            //     StopCoroutine(Decharge());
            //     StartCoroutine(Decharge());
            // }
            
        }
        

        
    }
    void Pulse(){
        //Color newColor = new Color(100,200,200, 255);

        
    
        child.color = new Color32(100,100,255,255);
    }
    IEnumerator Decharge(){
        yield return new WaitForSeconds(10);
        CurrentPings--;
    }

    private void OnTriggerEnter2D(Collider2D obj){
        if (obj.gameObject.name == "Circle"){
            within = true;
        }
        
    }
    
    private void OnTriggerExit2D(Collider2D obj){
        if (obj.gameObject.name == "Circle"){
            within = false;
        }
    }
    
    
    
}
