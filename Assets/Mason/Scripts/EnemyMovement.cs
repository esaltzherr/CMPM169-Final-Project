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
    [SerializeField] private float stationRadius;
    [SerializeField] private float deactivateTime;

    [HideInInspector] public bool trapped;
    [HideInInspector] public MovementMode mode;
    private GameManager gMan;
    private Seeker seek;
    private AIPath path;
    private Light2D viewLight;
    private Transform player;
    private Station currentStation;
    private bool[] coroutinesRunning;

    private void Awake() {
        seek = GetComponent<Seeker>();
        path = GetComponent<AIPath>();
        viewLight = GetComponent<Light2D>();

        viewLight.enabled = false;
        mode = MovementMode.PATROL;
        patrolPointIndex = 0;

        coroutinesRunning = new bool[3];
    }

    private void Start() {
        gMan = GameManager.instance;
    }

    private void Update() {
        if(gMan.gameOver) {
            StopAllCoroutines();
            SetNewDestination(Vector3.positiveInfinity);
            return;
        }

        if(!player)
            player = gMan.player.transform;

        // don't move if trapped
        if(trapped) {
            EndCoroutine(0);
            return;
        }

        Vector2 closeVec = GetClosestStation();

        // start chase mode
        if(Vector2.Distance(player.position, transform.position) < player.GetComponentInChildren<Light2D>().pointLightOuterRadius) {
            mode = MovementMode.CHASE;
            SetNewDestination(player.position);
            EndCoroutine(1);
            StartCoroutine("StartChase");
        }
        else if(closeVec.y < stationRadius && !coroutinesRunning[0]) {
            mode = MovementMode.ALARM;
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
        // alarm mode
        else if(mode == MovementMode.ALARM && !coroutinesRunning[1]) {
            currentStation = gMan.stations[(int)closeVec.x];
            if(path.destination != currentStation.transform.position) {
                SetNewDestination(currentStation.transform.position);
                return; // there is a split second when remaining distance < 0, but path is proper
            }
    
            if(CloseToDestination(2))
                StartCoroutine("DeactivateStation");
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().ApplyDamage(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other) {
        mode = MovementMode.CHASE;
        SetNewDestination(player.position);
        EndCoroutine(1);
        StartCoroutine("StartChase");
    }

    private bool CloseToDestination(float closeDistance) {
        return path.remainingDistance < closeDistance;
    }

    private Vector2 GetClosestStation() {
        float closestDist = Mathf.Infinity;
        int closestIndex = 0;
        for(int i = 0; i < gMan.stations.Count; ++i) {
            float dist = Vector2.Distance(transform.position, gMan.stations[i].transform.position);
            if(gMan.stations[i].captured && dist < closestDist) {
                closestDist = dist;
                closestIndex = i;
            }
        }
        return new Vector2(closestIndex, closestDist);
    }

    private IEnumerator StartChase() {
        if(coroutinesRunning[0]) yield break;

        // chase player until light turns off
        coroutinesRunning[0] = true;
        StartCoroutine(LightControl(() => CloseToDestination(0.15f)));
        yield return new WaitUntil(() => viewLight.enabled == false);
        mode = currentStation == null ? MovementMode.PATROL : MovementMode.ALARM;
        coroutinesRunning[0] = false;
    }

    private IEnumerator DeactivateStation() {
        if(coroutinesRunning[1]) yield break;
        
        coroutinesRunning[1] = true;
        yield return new WaitForSeconds(deactivateTime);
        currentStation.Deactivate();
        currentStation = null;
        mode = MovementMode.PATROL;
        coroutinesRunning[1] = false;
    }

    private IEnumerator LightControl(System.Func<bool> offCond) {
        if(coroutinesRunning[2]) yield break;

        coroutinesRunning[2] = true;
        viewLight.enabled = true;
        yield return new WaitUntil(offCond);
        yield return new WaitForSeconds(lightDuration);
        viewLight.enabled = false;
        coroutinesRunning[2] = false;
    }

    private void EndCoroutine(int index) {
        if(index < 0 || index > 2) return;

        coroutinesRunning[index] = false;
        if(index == 0)
            StopCoroutine("StartChase");
        else if(index == 1)
            StopCoroutine("DeactivateStation");
        else if(index == 2)
            StopCoroutine("LightControl");
    }

    public void SetNewDestination(Vector3 destination) {
        seek.StartPath(transform.position, destination);
        path.destination = destination;
    }
}
