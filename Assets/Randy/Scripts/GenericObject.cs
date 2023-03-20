using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GenericObject : MonoBehaviour
{
    Light2D _light;
    Coroutine lightTimer;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
        _light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LightTimer()
    {
        yield return new WaitForSeconds(5.0f);
        _light.enabled = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if(lightTimer != null)
            StopCoroutine(lightTimer);
        _light.enabled = true;
        lightTimer = StartCoroutine(LightTimer());
    }
}
