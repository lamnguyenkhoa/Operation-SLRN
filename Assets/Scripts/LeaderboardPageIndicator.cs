using UnityEngine;

public class LeaderboardPageIndicator : MonoBehaviour
{
    private RectTransform imageTransform;
    public float popHeight = 50f; // the amount to move the image up and down
    public float popSpeed = 2f; // the speed of the pop animation
    private float startY; // the starting y position of the image

    private void Start()
    {
        // record the starting y position of the image
        imageTransform = GetComponent<RectTransform>();
        startY = imageTransform.localPosition.y;
    }

    private void Update()
    {
        // calculate the y position offset based on time
        float sine = Mathf.Sin(Time.time * popSpeed);
        float yOffset = (sine >= 0) ? popHeight : 0;

        // update the y position of the image
        Vector3 newPos = new Vector3(imageTransform.localPosition.x, startY + yOffset, imageTransform.localPosition.z);
        imageTransform.localPosition = newPos;
    }
}
