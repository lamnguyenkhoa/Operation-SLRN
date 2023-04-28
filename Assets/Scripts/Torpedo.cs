using UnityEngine;
using UnityEngine.U2D;

public class Torpedo : UnderwaterEntity
{
    private float lifetimeTimer;
    public float lifeTimeDuration = 6f;
    public float speed = 5f;
    public AudioClip explodeSfx;
    public GameObject explosionPulse;
    public GameObject owner;


    /// <summary>
    /// Should be called right after Instantiate
    /// </summary>
    /// <param name="normalizedDir"></param>
    public void SetDirection(Vector2 normalizedDir)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = normalizedDir * speed;
    }

    protected override void Update()
    {
        base.Update();
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer > lifeTimeDuration)
        {
            Destroy(this.gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {

        if (col.name == "SubmarineSprite")
        {
            GameManager.instance.SubmarineDamaged(1);
            Instantiate(explosionPulse, transform.position, Quaternion.identity);
        }
        else
        {
            if (col.gameObject == owner)
            {
                return;
            }
            // Explode if contact with red fish, other enemy submarine, border and obstacles
            if (col.GetComponent<EnemyWander>() ||
                col.GetComponent<EnemySubmarine>() ||
                col.GetComponent<SpriteShapeController>())
            {
                Instantiate(explosionPulse, transform.position, Quaternion.identity);
                Destroy(this.gameObject);

                // If submarine within distance, play some explosion sound
                float distance = Vector2.Distance(transform.position, GameManager.instance.submarine.position);
                if (distance <= 5f)
                {
                    AudioSource.PlayClipAtPoint(explodeSfx, Camera.main.transform.position, GameManager.instance.sfxVolume);
                }

                // Torpedo can also used against red fish and enemy submarine
                // Not against the proximity mine because it already included in proximity mine code
                if (col.GetComponent<EnemyWander>() || col.GetComponent<EnemySubmarine>())
                {
                    Destroy(col.gameObject);
                }

            }
        }
    }
}
