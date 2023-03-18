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

    [Header("Alarm")]
    [SerializeField] private float deactivateTime;

    [HideInInspector] public bool trapped;
    [HideInInspector] public MovementMode mode;
    private GameManager gMan;
    private Seeker seek;
    private AIPath path;
    private Light2D viewLight;
    private CircleCollider2D stationTrigger;
    private Transform player;
    private Station currentStation;

    private void Awake() {
        seek = GetComponent<Seeker>();
        path = GetComponent<AIPath>();
        viewLight = GetComponent<Light2D>();
        stationTrigger = GetComponent<CircleCollider2D>();

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
                SetNewDestination(patrolPoints[patrolPointIndex]);
            }
        }
        else if(mode == MovementMode.ALARM) {
            if(CloseToDestination(2)) {
                // TODO: start a coroutine to deactivate a station if close
                StartCoroutine("DeactivateStation");
            }
        }

        // chase players in range
        if(Vector2.Distance(player.position, transform.position) < player.GetComponentInChildren<Light2D>().pointLightOuterRadius) {
            mode = MovementMode.CHASE;
            SetNewDestination(player.position);
            StopCoroutine("DeactivateStation");
            StartCoroutine("BackToPatrol");
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().ApplyDamage(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Station station = other.GetComponent<Station>();
        if(other.tag == "Station" && station.captured) {
            print("yo");
            mode = MovementMode.ALARM;
            currentStation = station;
            SetNewDestination(other.transform.position);
        }
    }

    private void OnParticleCollision(GameObject other) {
        mode = MovementMode.CHASE;
        SetNewDestination(player.position);
        StartCoroutine("BackToPatrol");
    }

    private bool CloseToDestination(float closeDistance) {
        return path.remainingDistance < closeDistance;
    }

    private IEnumerator BackToPatrol() {
        // chase player until light turns off
        StartCoroutine(LightControl(() => CloseToDestination(0.15f)));
        yield return new WaitUntil(() => viewLight.enabled == false);
        mode = MovementMode.PATROL;
    }

    private IEnumerator DeactivateStation() {
        SetNewDestination(Vector3.positiveInfinity);
        yield return new WaitForSeconds(deactivateTime);
        currentStation.Deactivate();
        currentStation = null;
        mode = MovementMode.PATROL;
    }

    private IEnumerator LightControl(System.Func<bool> offCond) {
        viewLight.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(lightDuration);
        viewLight.enabled = false;
    }

    public void SetNewDestination(Vector3 destination) {
        seek.StartPath(transform.position, destination);
        path.destination = destination;
    }
}
