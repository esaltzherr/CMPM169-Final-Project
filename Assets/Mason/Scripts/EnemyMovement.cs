using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public enum MovementMode { PATROL, CHASE, ALARM}

    [Header("Patrol")]
    [SerializeField] private Vector3 patrolPoint1;
    [SerializeField] private Vector3 patrolPoint2;
    private Vector3 targetPatrolPoint;

    [SerializeField] private float duration;
    private Seeker seek;
    private AIPath path;
    private Light2D viewLight;
    private MovementMode mode;

    // Start is called before the first frame update
    void Start()
    {
        seek = GetComponent<Seeker>();
        path = GetComponent<AIPath>();
        viewLight = GetComponent<Light2D>();
        viewLight.enabled = false;
        mode = MovementMode.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == MovementMode.PATROL) {
            if(path.destination != patrolPoint1 && path.destination != patrolPoint2)
                path.destination = targetPatrolPoint;
        }
    }

    private bool CloseToDestination() {
        return path.remainingDistance < 0.1f;
    }

    private void OnParticleCollision(GameObject other)
    {
        Vector2 pos = other.GetComponentInParent<SimplePlayer>().lastPos;
        seek.StartPath(transform.position, pos);
        path.destination = pos;
        mode = MovementMode.CHASE;
        StartCoroutine(LightControl(CloseToDestination));
    }

    IEnumerator LightControl(System.Func<bool> offCond)
    {
        viewLight.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(duration);
        viewLight.enabled = false;
    }
}
