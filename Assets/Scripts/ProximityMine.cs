using UnityEngine;

public class ProximityMine : UnderwaterEntity
{
    public AudioClip explodeSfx;
    public GameObject explosionPulse;

    public void Explode()
    {
        GameManager.instance.SubmarineDamaged(100);
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        // Install kill if within range
        if (col.name == "SubmarineSprite")
        {
            Explode();
        }
        else
        {
            // Mine can also used against enemy
            if (col.GetComponent<EnemyWander>() ||
                col.GetComponent<Torpedo>() ||
                col.GetComponent<EnemySubmarine>())
            {
                Instantiate(explosionPulse, transform.position, Quaternion.identity);
                Destroy(col.gameObject);
                Destroy(this.gameObject);

                // If submarine within distance, play some explosion sound
                float distance = Vector2.Distance(transform.position, GameManager.instance.submarine.position);
                if (distance <= 5f)
                {
                    AudioSource.PlayClipAtPoint(explodeSfx, Camera.main.transform.position, GameManager.instance.sfxVolume);
                }
            }
        }
    }
}
