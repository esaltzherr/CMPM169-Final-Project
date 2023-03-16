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
    public int hp;
    [SerializeField] float damageCooldown;
    [SerializeField] float pushForce;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetAngle();
        pivot.transform.rotation = Quaternion.Euler(0, 0, angle);
        ApplyMove();
    }

    public void PingDirectional(InputAction.CallbackContext context)
    {
        if (!ready) return;
        if (context.started)
        {
            StartCoroutine(PingCooldown());
            directionalParticle.Emit(40);
        }
    }

    public void PingArea(InputAction.CallbackContext context)
    {
        if (!ready) return;
        if (context.started)
        {
            StartCoroutine(PingCooldown());
            areaParticle.Emit(720);
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
        _rb.velocity = moveDirection * moveSpeed;
    }

    public void applyDamage(GameObject other)
    {
        Vector2 push = new Vector2(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
        push.Normalize();
        _rb.AddForce(push * pushForce, ForceMode2D.Impulse);
        if (!vulnerable) return;
        hp--;
        DamageCooldown();
    }

    private IEnumerator DamageCooldown()
    {
        vulnerable = false;
        yield return new WaitForSeconds(damageCooldown);
        vulnerable = true;
    }
}
