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

   public float fadeDuration = 1f; // Time to fully fade out
    public float fadeSpeed = 10f;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;
    private bool isFadingOut = false;
    private float fadeTimer = 0f;


    protected virtual void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        destinationPoint = patrolPoints[1].transform;
    }


    public void Dying()
    {
        Debug.Log("In Light");
        if (!isFadingOut)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    public void Reviving()
    {
        Debug.Log("Not In Light");
        if (isFadingOut)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut()
    {
        isFadingOut = true;
        fadeTimer = 0f;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            SetAlpha(alpha);

            if (!isFadingOut) yield break; // If reversed, stop fading out
            yield return null;
        }

        Die();
    }

    private IEnumerator FadeIn()
    {
        isFadingOut = false;
        fadeTimer = Mathf.Clamp(fadeTimer, 0f, fadeDuration); // Prevent overfading

        while (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(0f, 1f, 1 - (fadeTimer / fadeDuration));
            SetAlpha(alpha);

            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public void Die()
    {
        Debug.Log("Player Died");
        // Add your player death logic here (e.g., reload level, disable player, etc.)
        gameObject.SetActive(false);
    }


    protected virtual void FixedUpdate() {
        Patrol();
    }

    protected virtual void Patrol() {
        if (!isFadingOut) {
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