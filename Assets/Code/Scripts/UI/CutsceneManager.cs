using AudioSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cutsceneObject;
    [SerializeField] private TextMeshProUGUI expositionText;
    [SerializeField] private float sceneWaitTime;
    [SerializeField] private soundEffect slideSFX;
    [SerializeField] private soundEffect screamsSFX;

    public void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
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
            if (count == 1) EchosCry.Sound.PlaySFX(screamsSFX, this.transform, 0);
            EchosCry.Sound.PlaySFX(slideSFX, this.transform, 0);
            expositionText.text = cutsceneObject[count].GetComponent<CutsceneText>().Text;
            StartCoroutine(FadeInImage(count));
            count++;
            yield return new WaitForSeconds(sceneWaitTime);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Tutorial);
    }
    IEnumerator FadeInImage(int image)
    {
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            cutsceneObject[image].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }
}
