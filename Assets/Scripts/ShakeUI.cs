using Cinemachine;
using UnityEngine;


public class ShakeUI : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public GameObject submarineUI;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = submarineUI.transform.localPosition;
    }

    void Update()
    {
        if (vcam != null)
        {
            var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                // set the submarineUI position to a random position within the noise range
                // Sync with CinemachineShake.cs
                Vector2 noiseOffset = noise.m_AmplitudeGain * Random.insideUnitCircle;
                Vector3 newPosition = originalPosition + new Vector3(noiseOffset.x, noiseOffset.y, 0f);
                submarineUI.transform.localPosition = newPosition;
            }
        }
    }
}