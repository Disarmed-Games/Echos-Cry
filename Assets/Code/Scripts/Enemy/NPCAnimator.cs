using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class NPCAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _npcSprite;
    [SerializeField] Animator _animator;
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private ParticleSystem _staggerParticles;

    public SpriteRenderer NPCSprite { get { return _npcSprite; } }

    private Color _defaultTintColor;
    private readonly int hashedTintColor = Shader.PropertyToID("_TintColor");
 
    public void TintFlash(Color tintColor, float flashDuration)
    {
        StopCoroutine(TintFlashCoroutine(tintColor, flashDuration));
        StartCoroutine(TintFlashCoroutine(tintColor, flashDuration));
    }

    public void UpdateSpriteDirection(Vector3 direction)
    {
        if (direction.x == 0) return;

        Vector3 currentScale = _spriteTransform.localScale;
        currentScale.x = Mathf.Sign(direction.x) * Mathf.Abs(currentScale.x);
        _spriteTransform.localScale = currentScale;
    }
    public void PlayAnimation(int hashCode)
    {
        if (!_animator.HasState(0, hashCode)) return; 
        _animator.Play(hashCode);
    }

    public void PlayVisualEffect()
    {
        _visualEffect.Play();
    }

    public void StaggerParticleStart()
    {
        _staggerParticles.Play();
    }
    public void StaggerParticleStop()
    {
        _staggerParticles.Stop();
    }

    private IEnumerator TintFlashCoroutine(Color tintColor, float flashDuration)
    {
        _npcSprite.material.SetColor(hashedTintColor, tintColor);
        yield return new WaitForSeconds(flashDuration);
        _npcSprite.material.SetColor(hashedTintColor, _defaultTintColor);
    }

    private void Awake()
    {
        if (_npcSprite != null)
            _defaultTintColor = _npcSprite.material.GetColor(hashedTintColor);        
    }

    private void OnEnable()
    {
        _npcSprite.material.SetColor(hashedTintColor, _defaultTintColor);
    }
}

namespace EchosCry.Enemy.Animation
{
    public class HashCodes
    {
        public static readonly int MoveHashCode = Animator.StringToHash("Move");
        public static readonly int IdleHashCode = Animator.StringToHash("Idle");
        public static readonly int AttackHashCode = Animator.StringToHash("Attack");
    }

}