using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    public float moveSpeed = 2f;  // the speed at which the enemy moves
    public float rotateSpeed = 100f;  // the speed at which the enemy rotates
    public float minTime = 1f;  // the minimum time the enemy will walk in one direction
    public float maxTime = 4f;  // the maximum time the enemy will walk in one direction

    private float timer;  // the timer to keep track of how long the enemy has been walking in one direction
    private Vector2 direction;  // the direction the enemy is currently walking in
    private Rigidbody2D rb;  // the rigidbody component of the enemy
    public GameObject pulseEffectPrefab;

    void Start()
    {
        timer = Random.Range(minTime, maxTime);  // set the initial timer to a random value
        direction = Random.insideUnitCircle.normalized;  // set the initial direction to a random vector of length 1
        rb = GetComponent<Rigidbody2D>();  // get the rigidbody component
    }

    void FixedUpdate()
    {
        // move the enemy in the current direction
        Vector2 velocity = direction * moveSpeed;
        rb.velocity = velocity;

        // rotate the enemy to face the current direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float targetAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        // decrement the timer and change direction if the timer has expired
        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            direction = Random.insideUnitCircle.normalized;  // set the new direction to a random vector of length 1
            timer = Random.Range(minTime, maxTime);  // reset the timer to a random value
        }
    }

    public void Pulse()
    {
        Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
    }
}