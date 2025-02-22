using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSpriteColorRed()
    {
        if (spriteRenderer != null)
        {
            Debug.Log("Changing sprite color to red");
            spriteRenderer.color = Color.red;
        }
    }

    public void ChangeSpriteColorWhite()
    {
        if (spriteRenderer != null)
        {
            Debug.Log("Changing sprite color to white");
            spriteRenderer.color = Color.white;
        }
    }
}
