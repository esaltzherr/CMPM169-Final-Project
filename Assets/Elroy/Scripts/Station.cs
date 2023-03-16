using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] int radius;
    [SerializeField] int ClaimRadius;
    [SerializeField] int CapturePings;
    [SerializeField] int CurrentPings;
    [SerializeField] int DeChargeRate;
    [SerializeField] Color ClaimedColor;
    [SerializeField] Color UnClaimedColor;

    public bool within = false;
    // 0.4375
    public bool captured = false;

    SpriteRenderer child;
    SpriteRenderer station;
    
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).GetComponent<SpriteRenderer>();
        child.transform.localScale = new Vector3(ClaimRadius, ClaimRadius, 0.0f);
        station = GetComponent<SpriteRenderer>();
        GameManager.instance.stations.Add(this);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    IEnumerator Decharge(){
        yield return new WaitForSeconds(5);
        CurrentPings--;
        
    }

    void OnParticleCollision(GameObject other) {
        Transform player = other.transform.parent;
        print(Vector2.Distance(player.position, transform.position));
        if(player.tag == "Player" && Vector2.Distance(player.position, transform.position) < ClaimRadius) {
            CurrentPings++;
            StopCoroutine(Decharge());
            StartCoroutine(Decharge());
            if(CurrentPings >= CapturePings){
                //Change Color to green to show captured.
                station.color = ClaimedColor;
                captured = true;
                StopCoroutine(Decharge());
            }
            
        }
    }
    
    
}
