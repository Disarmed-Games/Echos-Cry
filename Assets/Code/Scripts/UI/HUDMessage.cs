using System.Collections;
using TMPro;
using UnityEngine;

public class HUDMessage : MonoBehaviour
{
    public static HUDMessage Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void UpdateMessage(string message, float messageDuration)
    {
        textMessage.text = message;
        StopAllCoroutines();
        StartCoroutine(LerpMessageAlpha(0f, 1f));
        StartCoroutine(MessageDuration(messageDuration));
    }

    private IEnumerator MessageDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        StartCoroutine(LerpMessageAlpha(1f, 0f));
    }

    private IEnumerator LerpMessageAlpha(float currentAlpha, float targetAlpha)
    {
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / 1f;
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, t);
            yield return null;
        }
    }
}
