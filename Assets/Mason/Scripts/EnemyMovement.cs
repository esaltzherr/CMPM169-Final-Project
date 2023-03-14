using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public enum MovementMode { PATROL, CHASE, ALARM }

    [Header("Patrol")]
    [SerializeField] private List<Vector3> patrolPoints;
    private int patrolPointIndex;

    [Header("Chase")]
    [SerializeField] private float duration;

    private GameManager gMan;
    private Seeker seek;
    private AIPath path;
    private Light2D viewLight;
    private MovementMode mode;

    private void Awake() {
        seek = GetComponent<Seeker>();
        path = GetComponent<AIPath>();
        viewLight = GetComponent<Light2D>();
        viewLight.enabled = false;
        mode = MovementMode.PATROL;
        patrolPointIndex = 0;
    }

    private void Start() {
        gMan = GameManager.instance;
    }

    private void Update() {
        // patrol mode
        if(mode == MovementMode.PATROL) {
            // transition from other mode to patrol mode
            if(!patrolPoints.Contains(path.destination))
                path.destination = patrolPoints[patrolPointIndex];
            
            // switch patrol points when close
            if(CloseToDestination(0.15f)) {
                patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Count;
                Vector3 pos = patrolPoints[patrolPointIndex];
                seek.StartPath(transform.position, pos);
                path.destination = pos;
            }
        }
        else if(mode == MovementMode.CHASE) {
            if(gMan.player == null) return;

            // keep chasing player if in player view radius
            Transform player = gMan.player.transform;
            if(Vector2.Distance(player.position, transform.position) < player.GetComponentInChildren<Light2D>().pointLightOuterRadius) {
                Vector3 pos = gMan.player.transform.position;
                seek.StartPath(transform.position, pos);
                path.destination = pos;
            }
        }
    }

    private bool CloseToDestination(float closeDistance) {
        return path.remainingDistance < closeDistance;
    }

    private void OnParticleCollision(GameObject other) {
        // get player's last known position and chase to it
        Vector2 pos = other.GetComponentInParent<SimplePlayer>().lastPos;
        seek.StartPath(transform.position, pos);
        path.destination = pos;
        StartCoroutine(Chase());
    }

    private IEnumerator Chase() {
        // chase player until light turns off
        mode = MovementMode.CHASE;
        StartCoroutine(LightControl(() => CloseToDestination(0.15f)));
        yield return new WaitUntil(() => viewLight.enabled == false);
        mode = MovementMode.PATROL;
    }

    private IEnumerator LightControl(System.Func<bool> offCond) {
        viewLight.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(duration);
        viewLight.enabled = false;
    }
}
