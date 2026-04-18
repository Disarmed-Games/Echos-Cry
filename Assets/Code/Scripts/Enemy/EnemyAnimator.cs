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

    public SpriteRenderer EnemySprite { get { return _enemySprite; } }

    private Color _defaultTintColor;
    private readonly int hashedTintColor = Shader.PropertyToID("_TintColor");
 
    public void TintFlash(Color tintColor, float flashDuration)
    {
        StartCoroutine(TintFlashCoroutine(tintColor, flashDuration));
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

    private IEnumerator TintFlashCoroutine(Color tintColor, float flashDuration)
    {
        _enemySprite.material.SetColor(hashedTintColor, tintColor);
        yield return new WaitForSeconds(flashDuration);
        _enemySprite.material.SetColor(hashedTintColor, _defaultTintColor);
    }

    private void Awake()
    {
        if (_enemySprite != null)
            _defaultTintColor = _enemySprite.material.GetColor(hashedTintColor);        
    }

    private void OnEnable()
    {
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