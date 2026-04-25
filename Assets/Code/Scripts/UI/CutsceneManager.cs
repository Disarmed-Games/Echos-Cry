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
            yield return new WaitForSeconds(sceneWaitTime);
            StartCoroutine(FadeOutImage(count));
            count++;
        }
    }
    IEnumerator FadeOutImage(int image)
    {
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            cutsceneObject[image].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        expositionText.text = cutsceneObject[image].GetComponent<CutsceneText>().Text;
    }
}
