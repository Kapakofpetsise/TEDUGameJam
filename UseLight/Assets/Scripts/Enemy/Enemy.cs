using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb2d;
    [SerializeField] protected new Collider2D collider;
    [SerializeField] protected Animator animator;
    protected const float deathAnimationTime = 0.5f;
    [SerializeField] protected GameObject[] patrolPoints;
    protected Transform destinationPoint;
    [SerializeField] protected float velocity;
    [SerializeField] protected float lightExposureTime;
    protected float currentExposureTime = 0f;
    protected bool isExposedToLight = false;
    protected Coroutine stunCoroutine;

    [Header("Events")]
    [Space]
    [SerializeField] protected UnityEvent OnDeathEvent;

    protected virtual void Start() {
        if (OnDeathEvent == null) {
            OnDeathEvent = new UnityEvent();
        }
        rb2d = GetComponent<Rigidbody2D>();
        destinationPoint = patrolPoints[1].transform;
    }

    public virtual void Die() {
        OnDeathEvent.Invoke();
    }

    protected virtual void FixedUpdate() {
        Patrol();
    }

    protected virtual void Patrol() {
        if (!isExposedToLight) {
            if (destinationPoint == patrolPoints[0].transform) {
                if (gameObject.CompareTag("EnemyA")) {
                    rb2d.linearVelocity = new Vector2(velocity, rb2d.linearVelocity.y);
                }
                if (gameObject.CompareTag("EnemyB")) {
                    rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, -velocity);
                }
                if (Vector2.Distance(transform.position, destinationPoint.position) < 0.5f) {
                    Flip();
                    destinationPoint = patrolPoints[1].transform;
                }
            } else if (destinationPoint == patrolPoints[1].transform) {
                if (gameObject.CompareTag("EnemyA")) {
                    rb2d.linearVelocity = new Vector2(-velocity, rb2d.linearVelocity.y);
                }
                if (gameObject.CompareTag("EnemyB")) {
                    rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, velocity);
                }

                if (Vector2.Distance(transform.position, destinationPoint.position) < 0.5f) {
                    Flip();
                    destinationPoint = patrolPoints[0].transform;
                }
            }
        }
    }

    protected virtual void Flip() {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void OnDie() {
        if (collider != null) {
            rb2d.bodyType = RigidbodyType2D.Static;
            collider.enabled = false;
            animator.SetTrigger("die");
            Destroy(gameObject, deathAnimationTime);
        }
    }

    public void ExposeToLight(bool exposed) {
        if (exposed) {
            if (!isExposedToLight) {
                isExposedToLight = true;
                rb2d.gravityScale = 0; // allows not to stuck enemies moving up and down (the enemy having EnemyB tag in this case) 
                stunCoroutine = StartCoroutine(StunAndDieCoroutine());
            }
        } else {
            isExposedToLight = false;
            StopCoroutine(stunCoroutine);
            currentExposureTime = 0f;
        }
    }

    private IEnumerator StunAndDieCoroutine() {
        rb2d.linearVelocity = Vector2.zero;
        while (currentExposureTime < lightExposureTime) {
            if (!isExposedToLight) {
                yield break;
            }
            currentExposureTime += Time.deltaTime;
            yield return null;
        }
        Die();
    }

    protected virtual void OnDrawGizmos() {
        for (int i = 0; i < patrolPoints.Length; i++) {
            Gizmos.DrawWireSphere(patrolPoints[i].transform.position, 0.5f);
            if (i != patrolPoints.Length - 1) {
                Gizmos.DrawLine(patrolPoints[i].transform.position, patrolPoints[i + 1].transform.position);
            } else {
                Gizmos.DrawLine(patrolPoints[i].transform.position, patrolPoints[0].transform.position);
            }
        }
    }
}
