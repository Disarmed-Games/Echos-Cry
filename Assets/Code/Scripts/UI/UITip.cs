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
    private Transform selectionAnchor;

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
            Vector2 mousePos = selectionAnchor != null ? (Vector2)selectionAnchor.position :
                Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(Screen.width / 2f, Screen.height / 2f);
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
        selectionAnchor = null;
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

    public void StartMessage(string message, object owner, Transform anchor = null)
    {
        currentOwner = owner;
        selectionAnchor = anchor;

        textMessage.text = message;
        messagePanel.SetActive(true);
        messageActive = true;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(1f));
    }

    public void UpdateMessage(string message, object owner)
    {
        if (currentOwner == owner) textMessage.text = message;
    }

    public void StopMessage(object owner)
    {
        if (currentOwner != owner)
            return;

        currentOwner = null;
        selectionAnchor = null;
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
