using UnityEngine;

public class SonarManager : MonoBehaviour
{
    public float rotationSpeed;
    private float totalRotationAngle = 0f;

    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.back * rotationAmount);
        totalRotationAngle += rotationAmount;

        if (totalRotationAngle >= 360f)
        {
            GameManager.instance.PlaySonarPing();
            totalRotationAngle = 0f;
        }
    }
}
