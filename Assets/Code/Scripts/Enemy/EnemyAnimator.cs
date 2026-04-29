using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _enemySprite;
    [SerializeField] Animator _animator;
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private ParticleSystem _staggerParticles;
    [SerializeField] private ParticleSystem _armorBreakingParticles;
    [SerializeField] private bool _isReversed;
    private Coroutine _coroutine;

    public SpriteRenderer EnemySprite { get { return _enemySprite; } }

    private Color _defaultTintColor = Color.white;
    private Color _currentTintColor = Color.white;
    private readonly int hashedTintColor = Shader.PropertyToID("_TintColor");
 
    public void TintFlash(Color tintColor, float flashDuration)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _enemySprite.material.SetColor(hashedTintColor, _currentTintColor);
        }
        _coroutine = StartCoroutine(TintFlashCoroutine(tintColor, flashDuration));
    }

    public void UpdateSpriteDirection(Vector3 direction)
    {
        if (direction.x == 0) return;

        Vector3 currentScale = _spriteTransform.localScale;
        currentScale.x = Mathf.Sign(direction.x) * Mathf.Abs(currentScale.x);
        if (_isReversed) currentScale.x *= -1;
        _spriteTransform.localScale = currentScale;
    }
    public void PlayAnimation(int hashCode)
    {
        if (!_animator.HasState(0, hashCode)) return; 
        _animator.Play(hashCode);
    }

    public void PlayArmorVisualEffect()
    {
        _armorBreakingParticles.Play();
    }

    public void PlayBloodVisualEffect()
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
    public void SetTint(Color tintColor)
    {
        _enemySprite.material.SetColor(hashedTintColor, tintColor);
        _currentTintColor = tintColor;
    }
    public void ResetTint()
    {
        _enemySprite.material.SetColor(hashedTintColor, _defaultTintColor);
        _currentTintColor = _defaultTintColor;
    }
    private IEnumerator TintFlashCoroutine(Color tintColor, float flashDuration)
    {
        Color currentColor = _enemySprite.material.GetColor(hashedTintColor);
        _enemySprite.material.SetColor(hashedTintColor, tintColor);
        yield return new WaitForSeconds(flashDuration);
        _enemySprite.material.SetColor(hashedTintColor, currentColor);
        _coroutine = null;
    }

    private void OnEnable()
    {
        _currentTintColor = Color.white;
        _enemySprite.material.SetColor(hashedTintColor, _defaultTintColor);
    }

    public class HashCodes
    {
        public static readonly int MoveHashCode = Animator.StringToHash("Move");
        public static readonly int IdleHashCode = Animator.StringToHash("Idle");
        public static readonly int AttackHashCode = Animator.StringToHash("Attack");
        public static readonly int FuseHashCode = Animator.StringToHash("Fuse");
        public static readonly int StaggerHashCode = Animator.StringToHash("Stagger");
    }
}