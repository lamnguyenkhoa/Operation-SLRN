using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SonarManager : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
    }
}
