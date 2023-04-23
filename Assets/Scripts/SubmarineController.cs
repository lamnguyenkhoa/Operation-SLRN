using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class SubmarineController : MonoBehaviour
{
    public float moveSpeed;
    public float normalSpeed;
    public float engineOffSpeed;

    private Rigidbody2D rb;
    private float moveHorizontal;
    private float moveVertical;
    public TextMeshProUGUI coordText;
    public Light2D[] buttonLights;
    public Light2D sonarLight;
    public Light2D engineLight;
    public bool sonarEnable = true;
    public bool engineEnable = true;
    public GameObject surroundLight;
    public GameObject sonarRotate;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = normalSpeed;
    }

    void Update()
    {
        coordText.text = $"X:{transform.position.x}    Y:{transform.position.y}";

        if (Input.GetKeyDown(KeyCode.Q))
        {
            sonarEnable = !sonarEnable;
            sonarLight.volumeIntensityEnabled = sonarEnable;
            sonarRotate.SetActive(sonarEnable);
            if (sonarEnable)
            {
                GameManager.instance.sonarBgs.Play();
            }
            else
            {
                GameManager.instance.sonarBgs.Stop();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            engineEnable = !engineEnable;
            engineLight.volumeIntensityEnabled = engineEnable;
            surroundLight.SetActive(engineEnable);
            if (engineEnable)
            {
                moveSpeed = normalSpeed;
            }
            else
            {
                moveSpeed = engineOffSpeed;
            }
        }
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
