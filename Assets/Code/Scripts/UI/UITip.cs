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
    private Coroutine currentRoutine;
    private object currentOwner;

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

    public void StopAllMessage()
    {
        currentOwner = null;
        messageActive = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        StopAllCoroutines();

        canvasGroup.alpha = 0f;
        messagePanel.SetActive(false);
    }

    public void StartMessage(string message, object owner)
    {
        currentOwner = owner;

        textMessage.text = message;
        messagePanel.SetActive(true);
        messageActive = true;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(1f));
    }

    public void StopMessage(object owner)
    {
        if (currentOwner != owner)
            return;

        currentOwner = null;
        messageActive = false;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.5f;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            messagePanel.SetActive(false);
    }
}
