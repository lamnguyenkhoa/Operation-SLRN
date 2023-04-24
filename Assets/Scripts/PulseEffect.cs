using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float scaleSpeed = 1f;
    public float alphaSpeed = 1f;
    public float startScale = 1f;
    public float targetScale = 4f;
    public float startAlpha = 1f;
    public float targetAlpha = 0f;

    private float currentScale;
    private float currentAlpha;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentScale = startScale;
        currentAlpha = startAlpha;
    }

    private void Update()
    {
        // Update scale
        currentScale = Mathf.MoveTowards(currentScale, targetScale, scaleSpeed * Time.deltaTime);
        spriteRenderer.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        // Update alpha
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaSpeed * Time.deltaTime);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, currentAlpha);

        // Destroy object when animation is complete
        if (currentScale == targetScale && currentAlpha == targetAlpha)
        {
            Destroy(gameObject);
        }
    }
}