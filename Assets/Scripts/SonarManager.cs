using UnityEngine;

public class SonarManager : MonoBehaviour
{
    public float rotationSpeed;
    private float totalRotationAngle = 0f;

    void Update()
    {
        // For every sonarLevel beyond 1, +25% rotate speed;
        float bonus = 1 + (float)((GameManager.instance.sonarLevel - 1) * 0.25);
        float rotationAmount = rotationSpeed * bonus * Time.deltaTime;
        transform.Rotate(Vector3.back * rotationAmount);
        totalRotationAngle += rotationAmount;

        if (totalRotationAngle >= 360f)
        {
            GameManager.instance.PlaySonarPing();
            totalRotationAngle = 0f;
        }
    }
}
