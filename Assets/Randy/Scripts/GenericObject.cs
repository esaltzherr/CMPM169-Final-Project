using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GenericObject : MonoBehaviour
{
    Light2D _light;
    [SerializeField] float duration;

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
        yield return new WaitForSeconds(duration);
        _light.enabled = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        //if (_light.enabled) return;
        _light.enabled = true;
        StartCoroutine(LightTimer());
    }
}
