using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Trap Stats")]
    [SerializeField] private float trapTime;
    [SerializeField] private float trapCooldown;

    private BoxCollider2D trigger;
    private bool trapActive;

    private void Awake() {
        trigger = GetComponent<BoxCollider2D>();
        trapActive = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(GameManager.instance.gameOver) {
            StopAllCoroutines();
            return;
        }

        if(other.tag == "Enemy" && trapActive)
            StartCoroutine(TrapEnemy(other.gameObject));
    }

    private IEnumerator TrapEnemy(GameObject enemy) {
        EnemyMovement move = enemy.GetComponentInParent<EnemyMovement>();

        // trap enemy
        move.trapped = true;
        move.SetNewDestination(transform.position);
        yield return new WaitForSeconds(trapTime);

        // un-trap enemy
        move.trapped = false;
        move.mode = EnemyMovement.MovementMode.PATROL;
        StartCoroutine(CooldownTrap());
    }
    
    private IEnumerator CooldownTrap() {
        trapActive = false;
        yield return new WaitForSeconds(trapCooldown);
        trapActive = true;
    }
}
