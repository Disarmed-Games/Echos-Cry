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

    private bool _skipSceneRequested = false;

    public void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        _inputTranslator.OnInteractEvent += RequestSkipScene;

        skipText.text = $"Press '{_inputTranslator.PlayerInputs.Gameplay.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"))}' to Skip";
    }
    public void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        _inputTranslator.OnInteractEvent -= RequestSkipScene;
    }

    private void RequestSkipScene()
    {
        _skipSceneRequested = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeThroughImages());
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Tutorial);
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
}
