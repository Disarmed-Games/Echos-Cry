using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private string _returnSceneName;

    public void BackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_returnSceneName);
    }
}
