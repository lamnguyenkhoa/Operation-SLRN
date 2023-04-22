using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SonarManager : MonoBehaviour
{
    public float rotationSpeed;
    private Light2D sonarLight;

    void Start()
    {
        sonarLight = GetComponent<Light2D>();
    }

    void Update()
    {
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
    }

    // when enemies touch sonar scan, pulse them.
    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyWander enemy = col.gameObject.GetComponent<EnemyWander>();
        if (enemy)
        {
            enemy.Pulse();
        }
    }
}
