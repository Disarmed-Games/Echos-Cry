using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class AnimateMetronome : MonoBehaviour
{
    [SerializeField] private GameObject pendulumObject;
    private int swingDirection = 1;
    private float previousProgress = 0f;

    private void Update()
    {
        float progress = BeatManager.Instance.BeatProgress;

        float angle = Mathf.Sin(progress * Mathf.PI) * 45f * swingDirection;
        pendulumObject.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (progress < previousProgress)
        {
            swingDirection *= -1;
        }

        previousProgress = progress;
    }
}
