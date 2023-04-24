using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEntity : MonoBehaviour
{
    public GameObject pulseEffectPrefab;
    protected float pulseTimer; // This pulse timer is to prevent sonar pulse the same object multiple times within a short window.
    protected float timeUntilNextPulse = 0.3f;

    protected virtual void Update()
    {
        pulseTimer += Time.deltaTime;
        if (GameManager.instance.endedGame)
        {
            Destroy(this.gameObject, 1f);
        }
    }

    public virtual void Pulse(bool forced)
    {
        if (pulseTimer > timeUntilNextPulse || forced)
        {
            Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            pulseTimer = 0;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        SonarManager sonar = col.GetComponent<SonarManager>();
        if (sonar)
        {
            Pulse(false);
        }
    }
}
