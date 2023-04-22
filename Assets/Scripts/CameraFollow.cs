using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // the object the camera will follow
    public float smoothSpeed = 0.125f;  // the smoothness of camera movement
    public Vector3 offset;  // the offset of the camera from the target

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}