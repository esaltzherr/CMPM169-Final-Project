using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scan : MonoBehaviour
{
    AstarPath path;

    private void Awake() {
        path = GetComponent<AstarPath>();
    }

    private void Start() {
        path.Scan();
        AstarPath.active.Scan();
    }
}
