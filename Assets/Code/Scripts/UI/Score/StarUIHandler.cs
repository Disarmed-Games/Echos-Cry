using AudioSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarUIHandler : MonoBehaviour
{
    [SerializeField] private Image bloodImage;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private soundEffect unlockSFX;

    public void UnlockStar()
    {
        bloodImage.enabled = true;
        EchosCry.Sound.PlaySFX(unlockSFX, Player.Instance.transform, 0);
        StartCoroutine(StarAnimate());
    }

    public void ResetStar()
    {
        bloodImage.enabled = false;
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
