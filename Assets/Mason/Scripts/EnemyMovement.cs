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
    [SerializeField] private float lightDuration;

    [HideInInspector] public bool trapped;
    [HideInInspector] public MovementMode mode;
    private GameManager gMan;
    private Seeker seek;
    private AIPath path;
    private Light2D viewLight;
    Transform player;

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
        if(!player)
            player = gMan.player.transform;

        // don't move if trapped
        if(trapped) {
            StopCoroutine("BackToPatrol");
            return;
        }

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

        // chase players in range
        if(Vector2.Distance(player.position, transform.position) < player.GetComponentInChildren<Light2D>().pointLightOuterRadius) {
            mode = MovementMode.CHASE;
            CalculatePlayerPos();
            StartCoroutine("BackToPatrol");
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            //other.gameObject.GetComponent<PlayerController>().ApplyDamage(gameObject);
        }
    }

    private bool CloseToDestination(float closeDistance) {
        return path.remainingDistance < closeDistance;
    }

    private void CalculatePlayerPos() {
        Vector3 pos = player.position;
        seek.StartPath(transform.position, pos);
        path.destination = pos;
    }

    private void OnParticleCollision(GameObject other) {
        mode = MovementMode.CHASE;
        CalculatePlayerPos();
        StartCoroutine("BackToPatrol");
    }

    private IEnumerator BackToPatrol() {
        // chase player until light turns off
        StartCoroutine(LightControl(() => CloseToDestination(0.15f)));
        yield return new WaitUntil(() => viewLight.enabled == false);
        mode = MovementMode.PATROL;
    }

    private IEnumerator LightControl(System.Func<bool> offCond) {
        viewLight.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(lightDuration);
        viewLight.enabled = false;
    }

    public void MoveToTrap(Vector3 trapPos) {
        seek.StartPath(transform.position, trapPos);
        path.destination = trapPos;
    }
}
