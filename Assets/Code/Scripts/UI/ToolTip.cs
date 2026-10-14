using System.Collections;
using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private bool renderTextWithoutBloom;
    private RectTransform worldTextRect;
    private Canvas overlayCanvas;
    private CanvasGroup overlayGroup;
    private readonly Vector3[] worldCorners = new Vector3[4];
    public string text;
    [SerializeField] private InputTranslator bindingTranslator;
    [SerializeField] private string bindingAction;
    [SerializeField] private string bindingFormat;
    public float displayTime = 2f;
    private float fadeDuration = 0.4f;

    public void ConfigureBinding(InputTranslator translator, string actionName, string format)
    {
        bindingTranslator = translator;
        bindingAction = actionName;
        bindingFormat = format;
        RefreshBinding();
    }

    private void OnEnable()
    {
        if (bindingTranslator != null) bindingTranslator.OnBindingsChanged += RefreshBinding;
        Canvas.willRenderCanvases += PositionOverlayText;
        if (overlayCanvas != null) overlayCanvas.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (bindingTranslator != null) bindingTranslator.OnBindingsChanged -= RefreshBinding;
        Canvas.willRenderCanvases -= PositionOverlayText;
        if (overlayCanvas != null) overlayCanvas.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (overlayCanvas != null) Destroy(overlayCanvas.gameObject);
    }

    private void CreateTextOverlay()
    {
        // Keep the original rect as a world anchor; draw its text after post-processing.
        worldTextRect = textMeshPro.rectTransform;
        GameObject overlay = new GameObject("ToolTipTextOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        overlayCanvas = overlay.GetComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayGroup = overlay.GetComponent<CanvasGroup>();
        overlayGroup.blocksRaycasts = false;
        overlayGroup.interactable = false;
        overlayGroup.alpha = 0f;

        TextMeshProUGUI overlayText = Instantiate(textMeshPro, overlay.transform, false);
        textMeshPro.enabled = false;
        textMeshPro = overlayText;
        textMeshPro.raycastTarget = false;
        RectTransform rect = textMeshPro.rectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.sizeDelta = worldTextRect.rect.size;
    }

    private void PositionOverlayText()
    {
        if (overlayCanvas == null) return;
        Camera camera = Camera.main;
        if (camera == null)
        {
            overlayGroup.alpha = 0f;
            return;
        }

        overlayCanvas.targetDisplay = camera.targetDisplay;
        Vector3 screenPosition = camera.WorldToScreenPoint(worldTextRect.position);
        overlayGroup.alpha = screenPosition.z > 0f ? GetComponent<CanvasGroup>().alpha : 0f;
        worldTextRect.GetWorldCorners(worldCorners);
        Vector2 bottomLeft = camera.WorldToScreenPoint(worldCorners[0]);
        Vector2 topLeft = camera.WorldToScreenPoint(worldCorners[1]);
        Vector2 bottomRight = camera.WorldToScreenPoint(worldCorners[3]);
        Vector2 horizontal = bottomRight - bottomLeft;
        RectTransform rect = textMeshPro.rectTransform;
        rect.anchoredPosition = screenPosition;
        rect.localScale = new Vector3(horizontal.magnitude / Mathf.Max(0.001f, worldTextRect.rect.width),
            (topLeft - bottomLeft).magnitude / Mathf.Max(0.001f, worldTextRect.rect.height), 1f);
        rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(horizontal.y, horizontal.x) * Mathf.Rad2Deg);
    }

    private void RefreshBinding()
    {
        text = bindingTranslator == null ? bindingFormat : string.Format(bindingFormat,
            bindingTranslator.GetBindingDisplayString(bindingTranslator.PlayerInputs.asset.FindAction(bindingAction, true)));
        if (textMeshPro != null) textMeshPro.text = text;
    }

    void Start()
    {
        if (renderTextWithoutBloom) CreateTextOverlay();
        if (bindingTranslator != null) RefreshBinding();
        textMeshPro.text = text;
        GetComponent<CanvasGroup>().alpha = 0f;
        StartCoroutine(TipTimer(displayTime));
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator TipTimer(float time)
    {
        yield return FadeIn();
        yield return new WaitForSeconds(time);
        yield return FadeOut();
    }
}
