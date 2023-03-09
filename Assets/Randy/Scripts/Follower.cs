using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Pathfinding;

public class Follower : MonoBehaviour
{
    [SerializeField] Seeker _seeker;
    [SerializeField] AIPath _aipath;
    [SerializeField] Light2D _light;
    [SerializeField] float duration;


    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _aipath = GetComponent<AIPath>();
        _light = GetComponent<Light2D>();
        _light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnParticleCollision(GameObject other)
    {
        Vector2 pos = other.GetComponentInParent<SimplePlayer>().lastPos;
        _seeker.StartPath(transform.position, pos);
        _aipath.destination = pos;

        StartCoroutine(LightControl(() => _aipath.remainingDistance < 0.1f));
    }

    IEnumerator LightControl(System.Func<bool> offCond)
    {
        _light.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(duration);
        _light.enabled = false;
    }
}
