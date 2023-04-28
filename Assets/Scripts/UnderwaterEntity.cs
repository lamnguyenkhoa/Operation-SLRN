using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEntity : MonoBehaviour
{
    public GameObject pulseEffectPrefab;
    protected float pulseLimitTimer; // This pulse timer is to prevent sonar pulse the same object multiple times within a short window.
    protected float timeUntilNextPulse = 0.3f;
    public bool selfPulse;
    private float selfPulseTimer;
    private float selfPulseInterval = 2f;
    public float selfPulseIntervalMin;
    public float selfPulseIntervalMax;


    protected virtual void Start()
    {
        selfPulseInterval = Random.Range(selfPulseIntervalMin, selfPulseIntervalMax);
    }

    protected virtual void Update()
    {
        pulseLimitTimer += Time.deltaTime;
        if (GameManager.instance.endedGame)
        {
            Destroy(this.gameObject, 1f);
        }
        if (selfPulse)
        {
            selfPulseTimer += Time.deltaTime;
            if (selfPulseTimer > selfPulseInterval)
            {
                selfPulseTimer = 0f;
                selfPulseInterval = Random.Range(selfPulseIntervalMin, selfPulseIntervalMax);
                Pulse(false);
            }
        }

    }

    public virtual void Pulse(bool forced)
    {
        if (pulseLimitTimer > timeUntilNextPulse || forced)
        {
            Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            pulseLimitTimer = 0;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        SonarManager sonar = col.GetComponent<SonarManager>();
        if (sonar)
        {
            Pulse(true);
        }
    }
}
