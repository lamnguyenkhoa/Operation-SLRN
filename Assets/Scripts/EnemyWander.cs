using UnityEngine;

public enum EnemyMood
{
    WANDERING,
    TIRED,
    CHASING
}

public class EnemyWander : UnderwaterEntity
{
    public float moveSpeed = 2f;
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;

    private float rotateSpeed = 100f;  // the speed at which the enemy rotates
    public float minTime = 1f;  // the minimum time the enemy will walk in one direction
    public float maxTime = 4f;  // the maximum time the enemy will walk in one direction
    private float wanderTimer;  // the timer to keep track of how long the enemy has been walking in one direction.
    private Vector2 wanderDirection;  // the direction the enemy is currently walking in
    private Rigidbody2D rb;  // the rigidbody component of the enemy
    private EnemyMood mood;
    private float chaseTimer;
    public float timeUntilStopChase = 10f; // Enemy stop chasing if after this time
    public float distanceUntilStopChase = 10f; // Enemy stop chasing submarine outside this range
    private float tiredTimer;
    public float timeInTiredMood = 2f;

    void Start()
    {
        wanderTimer = Random.Range(minTime, maxTime);  // set the initial timer to a random value
        wanderDirection = Random.insideUnitCircle.normalized;  // set the initial direction to a random vector of length 1
        rb = GetComponent<Rigidbody2D>();  // get the rigidbody component
    }

    void FixedUpdate()
    {
        float angle;
        float targetAngle;

        switch (mood)
        {
            case EnemyMood.WANDERING:
                // move the enemy in the current direction
                rb.velocity = wanderDirection * moveSpeed;

                // rotate the enemy to face the current direction
                angle = Mathf.Atan2(wanderDirection.y, wanderDirection.x) * Mathf.Rad2Deg - 90f;
                targetAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, rotateSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

                // decrement the timer and change direction if the timer has expired
                wanderTimer -= Time.fixedDeltaTime;
                if (wanderTimer <= 0f)
                {
                    wanderDirection = Random.insideUnitCircle.normalized;  // set the new direction to a random vector of length 1
                    wanderTimer = Random.Range(minTime, maxTime);  // reset the timer to a random value

                    // Self pulse
                    Pulse();
                }
                break;
            case EnemyMood.CHASING:
                // calculate the direction to move towards the target
                Vector2 chaseDirection = (GameManager.instance.submarine.position - transform.position).normalized;

                // move towards the target
                rb.velocity = chaseDirection * moveSpeed;

                // rotate the enemy to face the current direction
                angle = Mathf.Atan2(chaseDirection.y, chaseDirection.x) * Mathf.Rad2Deg - 90f;
                targetAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, rotateSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

                chaseTimer += Time.fixedDeltaTime;
                if (chaseTimer > timeUntilStopChase)
                {
                    chaseTimer = 0f;
                    mood = EnemyMood.TIRED;
                    moveSpeed = 0f;
                }
                break;
            case EnemyMood.TIRED:
                tiredTimer += Time.fixedDeltaTime;
                if (tiredTimer > timeInTiredMood)
                {
                    tiredTimer = 0f;
                    wanderTimer = Random.Range(minTime, maxTime);
                    wanderDirection = Random.insideUnitCircle.normalized;
                    mood = EnemyMood.WANDERING;
                    moveSpeed = wanderSpeed;
                }
                break;
            default:
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        // Start chasing if submarine is close range with engine on.
        if (col.name == "SubmarineSprite")
        {
            if (col.transform.parent.GetComponent<SubmarineController>().engineEnable)
            {
                mood = EnemyMood.CHASING;
                moveSpeed = chaseSpeed;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.name == "SubmarineSprite")
        {
            GameManager.instance.SubmarineDamaged();
        }
    }
}