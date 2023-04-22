using UnityEngine;
using TMPro;

public class SubmarineController : MonoBehaviour
{
    public float moveSpeed; // The speed at which the player moves
    private Rigidbody2D rb;
    private float moveHorizontal;
    private float moveVertical;
    public TextMeshProUGUI coordText;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        coordText.text = $"X:{transform.position.x} Y:{transform.position.y}";
    }

    void FixedUpdate()
    {
        // Get the horizontal and vertical input axes
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        // Move the player based on the input axes
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * moveSpeed;
    }

}
