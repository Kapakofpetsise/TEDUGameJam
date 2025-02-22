using UnityEngine;
using System.Collections;
public class Player : MonoBehaviour
{
    public float fadeDuration = 3f; // Time to fully fade out
    public float fadeSpeed = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Coroutine fadeCoroutine;
    private bool isFadingOut = false;
    private float fadeTimer = 0f;

    void Start()
    {

    }

    public void InLight()
    {
        Debug.Log("Not In Light");
        if (!isFadingOut)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    public void NotInLight()
    {
        Debug.Log("In Light");
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

    private void Die()
    {
        Debug.Log("Player Died");
        // Add your player death logic here (e.g., reload level, disable player, etc.)
        gameObject.SetActive(false);
    }
}
