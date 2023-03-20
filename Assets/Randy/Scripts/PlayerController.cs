using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject pivot;
    [SerializeField] ParticleSystem directionalParticle;
    [SerializeField] ParticleSystem areaParticle;
    [SerializeField] float pingCooldown;
    [SerializeField] float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float velocityPow;
    [SerializeField] float damageCooldown;
    [SerializeField] float pushForce;
    public int maxHp;
    public int hp { get; private set; }

    Rigidbody2D _rb;

    [Header("Debug Info")]
    [SerializeField] Vector2 moveDirection;
    [SerializeField] float angle;
    [SerializeField] bool ready;
    [SerializeField] bool vulnerable;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        ready = true;
        vulnerable = true;
        hp = maxHp;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameOver)
        {
            StopAllCoroutines();
            return;
        }
        GetAngle();
        pivot.transform.rotation = Quaternion.Euler(0, 0, angle);
        ApplyMove();
    }

    public void PingDirectional(InputAction.CallbackContext context)
    {
        if (!ready || GameManager.instance.gameOver) return;
        if (context.started)
        {
            StartCoroutine(PingCooldown());
            directionalParticle.Emit(40);
            AudioManager.Instance.PlaySound(1);
        }
    }

    public void PingArea(InputAction.CallbackContext context)
    {
        if (!ready || GameManager.instance.gameOver) return;
        if (context.started)
        {
            StartCoroutine(PingCooldown());
            areaParticle.Emit(720);
            AudioManager.Instance.PlaySound(0);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void GetAngle()
    {
        Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouseXY.y - transform.position.y, mouseXY.x - transform.position.x) * Mathf.Rad2Deg;
    }

    private IEnumerator PingCooldown()
    {
        ready = false;
        yield return new WaitForSeconds(pingCooldown);
        ready = true;
    }

    private void ApplyMove()
    {
        Vector2 targetVelocity = moveDirection * moveSpeed;
        Vector2 speedDif = new Vector2(targetVelocity.x - _rb.velocity.x, targetVelocity.y - _rb.velocity.y);
        float accelRate = (Mathf.Abs(targetVelocity.x) > 0.01f && Mathf.Abs(targetVelocity.y) > 0.01f) ? acceleration : deceleration;
        Vector2 movement = new Vector3(Mathf.Pow(Mathf.Abs(speedDif.x) * accelRate, velocityPow) * Mathf.Sign(speedDif.x),
                                       Mathf.Pow(Mathf.Abs(speedDif.y) * accelRate, velocityPow) * Mathf.Sign(speedDif.y));
        _rb.AddForce(movement);
    }

    public void ApplyDamage(GameObject other)
    {
        Vector2 push = new Vector2(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
        push.Normalize();
        _rb.AddForce(push * pushForce, ForceMode2D.Impulse);
        if (!vulnerable) return;
        hp--;
        vulnerable = false;
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        vulnerable = true;
    }
}
