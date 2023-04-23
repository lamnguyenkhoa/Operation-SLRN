using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEntity : MonoBehaviour
{
    public GameObject pulseEffectPrefab;
    protected float pulseTimer;
    protected float timeUntilNextPulse = 0.5f;

    protected virtual void Update()
    {
        pulseTimer += Time.deltaTime;
    }

    public virtual void Pulse()
    {
        if (pulseTimer > timeUntilNextPulse)
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
            Pulse();
        }
    }
}
