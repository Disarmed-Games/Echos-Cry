using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UITip : MonoBehaviour
{
    public static UITip Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private RectTransform rectTransform;
    private bool messageActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (messageActive)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            this.transform.position = mousePos;

            if (mousePos.x < Screen.width / 2f)
            { //Left side of screen.
                rectTransform.pivot = new Vector2(0f, 0f);
            }
            else
            { //Right side of screen.
                rectTransform.pivot = new Vector2(1f, 0f);
            }
        }
    }

    public void StartMessage(string message)
    {
        textMessage.text = message;
        messageActive = true;
        messagePanel.SetActive(messageActive);
        StopAllCoroutines();
        StartCoroutine(LerpMessageAlpha(0f, 1f));
    }
    public void StopMessage()
    {
        StopAllCoroutines();
        StartCoroutine(LerpMessageAlpha(0f, 1f));
        StartCoroutine(EndMessageRoutine());
    }

    private IEnumerator EndMessageRoutine()
    {
        yield return StartCoroutine(LerpMessageAlpha(canvasGroup.alpha, 0f));
        messageActive = false;
        messagePanel.SetActive(messageActive);
    }

    private IEnumerator LerpMessageAlpha(float currentAlpha, float targetAlpha)
    {
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / 0.5f;
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, t);
            yield return null;
        }
    }
}
