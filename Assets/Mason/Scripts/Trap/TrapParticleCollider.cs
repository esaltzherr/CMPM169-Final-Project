using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapParticleCollider : MonoBehaviour
{
    [SerializeField] private int playerLayer;
    [SerializeField] private int enemyLayer;

    private void Awake() {
        Physics2D.IgnoreLayerCollision(gameObject.layer, playerLayer);
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer);
    }
}
