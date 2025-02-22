using UnityEngine;
using System.Collections;

public class FadeInAndOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    public bool fadeOutOnStart = true;

    void Start()
    {
        if (fadeOutOnStart)
        {
            canvasGroup.alpha = 1f;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(1f);
    }


    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
