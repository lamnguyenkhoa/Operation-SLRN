using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PulsingEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float pulseScale;
    public float minPulse, maxPulse;
    public float pulseSpeed;
    public float maxAlpha; // should be  0-1
    public float timer;
    public float restTime;
    public bool loop;
    public bool destroyAfterDone;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pulseScale = minPulse;
    }

    private void Update()
    {
        float velocity = 0f;
        transform.localScale = new Vector2(pulseScale, pulseScale);
        pulseScale = Mathf.SmoothDamp(pulseScale, maxPulse, ref velocity, 1 / pulseSpeed, 100, Time.deltaTime);

        Color color = spriteRenderer.color;
        color.a = maxAlpha - ((pulseScale - minPulse) / (maxPulse - minPulse));
        spriteRenderer.color = color;

        if (pulseScale >= (maxPulse - 0.05f))
        {
            if (timer < restTime)
                timer += Time.deltaTime;
            else
            {
                if (loop)
                {
                    timer = 0f;
                    pulseScale = minPulse;
                }
                else if (destroyAfterDone)
                {
                    Destroy(this.gameObject, 0.5f);
                }

            }
        }
    }

    public void Pulse()
    {
        timer = 0f;
        pulseScale = minPulse;
    }
}
