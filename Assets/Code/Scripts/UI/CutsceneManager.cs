using AudioSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private GameObject[] cutsceneObject;
    [SerializeField] private TextMeshProUGUI expositionText;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private float sceneWaitTime;
    [SerializeField] private soundEffect slideSFX;
    [SerializeField] private soundEffect screamsSFX;
    [SerializeField, Min(0.1f)] private float blinkCloseDuration = 1.5f;

    [SerializeField, Min(0.1f)] private float blinkOpenDuration = 3.5f;

    private bool _skipSceneRequested = false;

    public void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        _inputTranslator.OnInteractEvent += RequestSkipScene;

        _inputTranslator.OnBindingsChanged += RefreshBinding;
        RefreshBinding();
    }
    public void OnDisable()
    {
        _inputTranslator.OnBindingsChanged -= RefreshBinding;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        _inputTranslator.OnInteractEvent -= RequestSkipScene;
    }

    private void RequestSkipScene()
    {
        _skipSceneRequested = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RevealIntro());
        StartCoroutine(FadeThroughImages());
    }
    private IEnumerator RevealIntro()
    {
        GameObject overlay = CreateFadeOverlay();
        CanvasGroup group = overlay.GetComponent<CanvasGroup>();
        group.alpha = 1f;

        float elapsed = 0f;
        float duration = Mathf.Max(0.1f, blinkOpenDuration);
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = 1f - Mathf.SmoothStep(0f, 1f, elapsed / duration);
            yield return null;
        }
        Destroy(overlay);
    }
    IEnumerator FadeThroughImages()
    {
        int count = 0;

        while (count < cutsceneObject.Length)
        {
            if (count == 1)
                EchosCry.Sound.PlaySFX(screamsSFX, this.transform, 0);

            EchosCry.Sound.PlaySFX(slideSFX, this.transform, 0);

            expositionText.text =
                cutsceneObject[count].GetComponent<CutsceneText>().Text;

            CanvasGroup cg = cutsceneObject[count].GetComponent<CanvasGroup>();

            yield return FadeStep(cg);

            count++;
        }
        yield return CloseBlink();
    }
    private GameObject CreateFadeOverlay()
    {
        GameObject overlay = new GameObject("BlinkFade", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        Canvas canvas = overlay.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        CanvasGroup group = overlay.GetComponent<CanvasGroup>();
        group.alpha = 0f;

        GameObject imageObject = new GameObject("Black", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        imageObject.transform.SetParent(overlay.transform, false);
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        imageObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        return overlay;
    }
    private IEnumerator CloseBlink()
    {
        // Cover the entire intro, including its text, before loading the tutorial.
        GameObject overlay = CreateFadeOverlay();
        CanvasGroup group = overlay.GetComponent<CanvasGroup>();

        float elapsed = 0f;
        float duration = Mathf.Max(0.1f, blinkCloseDuration);
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            yield return null;
        }
        group.alpha = 1f;
        // Render a fully black frame before the scene swap.
        yield return null;
        DontDestroyOnLoad(overlay);
        overlay.AddComponent<TutorialBlinkReveal>().Begin(blinkOpenDuration);
    }
    IEnumerator FadeStep(CanvasGroup cg)
    {
        _skipSceneRequested = false;

        float elapsed = 0f;
        float duration = 1f;

        cg.alpha = 0f;

        while (elapsed < duration)
        {
            if (_skipSceneRequested)
            {
                cg.alpha = 1f;
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cg.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        cg.alpha = 1f;
        float timer = 0f;

        while (timer < sceneWaitTime)
        {
            if (_skipSceneRequested)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }
    private void RefreshBinding()
    {
        skipText.text = $"Press '{_inputTranslator.GetBindingDisplayString(_inputTranslator.PlayerInputs.Gameplay.Interact)}' to Skip";
    }
}

// Runs on the persistent overlay so destroying the intro cannot stop the reveal.
public class TutorialBlinkReveal : MonoBehaviour
{
    public void Begin(float duration)
    {
        StartCoroutine(LoadAndReveal(Mathf.Max(0.1f, duration)));
    }

    private IEnumerator LoadAndReveal(float duration)
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneNames.Tutorial);
        yield return new WaitForSecondsRealtime(1f);

        CanvasGroup group = GetComponent<CanvasGroup>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = 1f - Mathf.SmoothStep(0f, 1f, elapsed / duration);
            yield return null;
        }
        group.alpha = 0f;
        Destroy(gameObject);
    }
}
