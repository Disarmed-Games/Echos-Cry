using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarUIHandler : MonoBehaviour
{
    [SerializeField] private Image bloodImage;
    [SerializeField] private float duration = 0.5f;

    public void UnlockStar()
    {
        bloodImage.enabled = true;
        StartCoroutine(StarAnimate());
    }

    IEnumerator StarAnimate()
    {
        float time = 0f;
        bloodImage.fillAmount = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            bloodImage.fillAmount = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        bloodImage.fillAmount = 1f;
    }
}
