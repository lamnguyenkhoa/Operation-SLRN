using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class SubmarineController : MonoBehaviour
{
    public float moveSpeed; // The speed at which the player moves
    private Rigidbody2D rb;
    private float moveHorizontal;
    private float moveVertical;
    public TextMeshProUGUI coordText;
    public Light2D[] buttonLights;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        coordText.text = $"X:{transform.position.x}    Y:{transform.position.y}";
    }

    void FixedUpdate()
    {
        // Get the horizontal and vertical input axes
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        // Turn on button light according to movement for
        buttonLights[0].volumeIntensityEnabled = false;
        buttonLights[1].volumeIntensityEnabled = false;
        buttonLights[2].volumeIntensityEnabled = false;
        buttonLights[3].volumeIntensityEnabled = false;
        if (moveHorizontal < 0)
            buttonLights[0].volumeIntensityEnabled = true;
        else if (moveHorizontal > 0)
            buttonLights[1].volumeIntensityEnabled = true;
        if (moveVertical < 0)
            buttonLights[2].volumeIntensityEnabled = true;
        else if (moveVertical > 0)
            buttonLights[3].volumeIntensityEnabled = true;

        // Move the player based on the input axes
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * moveSpeed;
    }
}
