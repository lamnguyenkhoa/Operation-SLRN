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
    public GameObject upgradeScreen;
    public TextMeshProUGUI speedMeter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = normalSpeed;
    }

    void Update()
    {
        coordText.text = $"X:{transform.position.x}  Y:{transform.position.y}";

        if (!GameManager.instance.disableSubmarineControl)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !GameManager.instance.isInUpgradeMenu)
            {
                sonarEnable = !sonarEnable;
                sonarLight.volumeIntensityEnabled = sonarEnable;
                sonarRotate.SetActive(sonarEnable);
            }
            if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.isInUpgradeMenu)
            {
                engineEnable = !engineEnable;
                engineLight.volumeIntensityEnabled = engineEnable;
                if (engineEnable)
                {
                    float bonus = 1 + (float)((GameManager.instance.speedLevel - 1) * 0.15);
                    moveSpeed = normalSpeed * bonus;
                }
                else
                {
                    float bonus = 1 + (float)((GameManager.instance.speedLevel - 1) * 0.15);
                    moveSpeed = engineOffSpeed * bonus;
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && !GameManager.instance.isInUpgradeMenu)
            {
                GameManager.instance.isInUpgradeMenu = true;
                upgradeScreen.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.R) && GameManager.instance.isInUpgradeMenu)
            {
                GameManager.instance.isInUpgradeMenu = false;
                upgradeScreen.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        // Get the horizontal and vertical input axes
        if (!GameManager.instance.disableSubmarineControl && !GameManager.instance.isInUpgradeMenu)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }
        else
        {
            moveHorizontal = 0f;
            moveVertical = 0f;
        }


        // Turn on button light according to movement for
        buttonLights[0].intensity = 0;
        buttonLights[1].intensity = 0;
        buttonLights[2].intensity = 0;
        buttonLights[3].intensity = 0;
        if (moveHorizontal < 0)
            buttonLights[0].intensity = 1f;
        else if (moveHorizontal > 0)
            buttonLights[1].intensity = 1f;
        if (moveVertical < 0)
            buttonLights[2].intensity = 1f;
        else if (moveVertical > 0)
            buttonLights[3].intensity = 1f;

        // Move the player based on the input axes
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }
        rb.velocity = movement * moveSpeed;
        int speed = (int)Mathf.Round((float)rb.velocity.magnitude * 10);
        speedMeter.text = speed.ToString();
    }

    public void UpdateSpeedBonus()
    {
        // For every speedLevel beyond 1, +15% move speed;
        float bonus = 1 + (float)((GameManager.instance.speedLevel - 1) * 0.15);
        if (engineEnable)
            moveSpeed = normalSpeed * bonus;
        else
            moveSpeed = engineOffSpeed * bonus;
    }
}
