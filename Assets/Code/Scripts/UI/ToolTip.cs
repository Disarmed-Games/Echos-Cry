using System.Collections;
using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
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
    }

    private void OnDisable()
    {
        if (bindingTranslator != null) bindingTranslator.OnBindingsChanged -= RefreshBinding;
    }

    private void RefreshBinding()
    {
        text = bindingTranslator == null ? bindingFormat : string.Format(bindingFormat,
            bindingTranslator.GetBindingDisplayString(bindingTranslator.PlayerInputs.asset.FindAction(bindingAction, true)));
        if (textMeshPro != null) textMeshPro.text = text;
    }

    void Start()
    {
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
