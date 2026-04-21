using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationClip _clip;
    private readonly int animHash = Animator.StringToHash("Bounce");
    public TextMeshProUGUI TextMesh { get { return _textMesh; } }
    private void Start()
    {
        _animator.Play(animHash);
        StartCoroutine(DestroyAfterTime(_clip.length));
    }
    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(gameObject);
    }
}
