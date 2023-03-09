using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] int particleCount;
    [SerializeField] float speed;
    public Vector2 lastPos { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Ping();
        Vector3 movement = new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal"), speed * Time.deltaTime * Input.GetAxis("Vertical"));
        transform.position += movement;
    }

    void Ping()
    {
        lastPos = transform.position;
        _particleSystem.Emit(particleCount);
    }
}
