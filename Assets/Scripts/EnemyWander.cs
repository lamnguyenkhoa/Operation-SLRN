using UnityEngine;

public enum EnemyMood
{
    WANDERING,
    TIRED,
    CHASING
}

public class EnemyWander : MonoBehaviour
{
    public float moveSpeed = 2f;  // the speed at which the enemy moves
    private float rotateSpeed = 100f;  // the speed at which the enemy rotates
    public float minTime = 1f;  // the minimum time the enemy will walk in one direction
    public float maxTime = 4f;  // the maximum time the enemy will walk in one direction
    private float walkingTimer;  // the timer to keep track of how long the enemy has been walking in one direction
    private Vector2 direction;  // the direction the enemy is currently walking in
    private Rigidbody2D rb;  // the rigidbody component of the enemy
    public GameObject pulseEffectPrefab;
    private float pulseTimer;
    private float timeUntilNextPulse = 0.5f;
    private EnemyMood mood;
    private float chaseTimer;
    public float timeUntilStopChase = 10f; // Enemy stop chasing if after this time
    public float distanceUntilStopChase = 10f; // Enemy stop chasing submarine outside this range

    private float tiredTimer;
    public float timeInTiredMood = 2f;


    void Start()
    {
        walkingTimer = Random.Range(minTime, maxTime);  // set the initial timer to a random value
        direction = Random.insideUnitCircle.normalized;  // set the initial direction to a random vector of length 1
        rb = GetComponent<Rigidbody2D>();  // get the rigidbody component
    }

    void Update()
    {
        pulseTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (mood == EnemyMood.WANDERING)
        {
            // move the enemy in the current direction
            Vector2 velocity = direction * moveSpeed;
            rb.velocity = velocity;

            // rotate the enemy to face the current direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float targetAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            // decrement the timer and change direction if the timer has expired
            walkingTimer -= Time.fixedDeltaTime;
            if (walkingTimer <= 0f)
            {
                direction = Random.insideUnitCircle.normalized;  // set the new direction to a random vector of length 1
                walkingTimer = Random.Range(minTime, maxTime);  // reset the timer to a random value
            }
        }
        else
        {

        }

    }

    public void Pulse()
    {
        if (pulseTimer > timeUntilNextPulse)
        {
            Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            pulseTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        SubmarineController submarine = col.gameObject.GetComponent<SubmarineController>();
        if (submarine)
        {
            Debug.Log("Detected");
        }
    }
}