using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Configuration Object")]
    [SerializeField] private PlayerAnimatorConfig _playerAnimatorConfig;
    [Header("Animator System Dependencies")]
    [SerializeField] private TrailRenderer  _dashTrail;
    [SerializeField] private Transform      _playerMainSpriteTransform;
    [SerializeField] private SpriteRenderer _playerMainSpriteRenderer;
    [SerializeField] private Animator       _playerMainSpriteAnimator;
    [SerializeField] private ParticleSystem _noteParticles;
    public Animator SpriteAnimator { get => _playerMainSpriteAnimator; }

    private Color defaultSpriteColor;

    private readonly int hashedTintColor = Shader.PropertyToID("_TintColor");

    private void Start()
    {
        defaultSpriteColor = _playerMainSpriteRenderer.material.GetColor(hashedTintColor);
    }
    public void StartMovementParticles()
    {
        _noteParticles.Play();
    }
    public void EndMovementParticles()
    {
        _noteParticles.Stop();
    }

    public void UpdateMainSpriteDirection(Vector2 locomotion)
    {
        if (locomotion.x == 0) return;

        Vector3 currentScale = _playerMainSpriteTransform.localScale;
        currentScale.x = Mathf.Sign(locomotion.x) * Mathf.Abs(currentScale.x);
        _playerMainSpriteTransform.localScale = currentScale;
    }

    public void TintFlash(Color tintColor)
    {
        StopCoroutine(TintFlashCoroutine(tintColor));
        StartCoroutine(TintFlashCoroutine(tintColor));
    }
    private IEnumerator TintFlashCoroutine(Color tintColor)
    {
        _playerMainSpriteRenderer.material.SetColor(hashedTintColor, tintColor);
        CameraManager.Instance.ScreenShake(.8f, .8f);

        yield return new WaitForSeconds(_playerAnimatorConfig.TintFlashDuration);
        _playerMainSpriteRenderer.material.SetColor(hashedTintColor, defaultSpriteColor);
    }

    public void SetIsTrailEmit(bool isEmitting)
    {
        _dashTrail.emitting = isEmitting;
    }
}