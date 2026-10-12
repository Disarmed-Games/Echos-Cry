using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthSystem _healthSystem;
    private PlayerDamageScreenFlash _damageScreenFlash;

    private void Awake()
    {
        _damageScreenFlash = gameObject.AddComponent<PlayerDamageScreenFlash>();
    }

    public bool HasArmor => _healthSystem.CurrentArmor > 0;
    public bool HasHealth => _healthSystem.CurrentHealth > 0;

    public float CurrentHealth => _healthSystem.CurrentHealth;
    public float CurrentArmor => _healthSystem.CurrentArmor;
    public float MaxHealth => _healthSystem.MaxHealth;
    public float MaxArmor => _healthSystem.MaxArmor;
    public bool IsInvincible { get => _healthSystem.IsInvincible; set => _healthSystem.IsInvincible = value; }

    [Header("(Player Only) Event Channels")]
    [SerializeField] DoubleFloatEventChannel _healthChannel;
    [SerializeField] DoubleFloatEventChannel _armorChannel;

    private float _regenHealthTickTime = 1.5f;
    private float _regenHealthAmount = 2f;
    private float _regenHealthDuration = 30f;
    private bool _canRegenHealth = false;
    private Coroutine _regenHealthTickCoroutine;
    public float RegenHealthAmount { get => _regenHealthAmount; set => _regenHealthAmount = value; }

    private float _regenArmorTickTime = 2f;
    private float _regenArmorAmount = 1f;
    private float _regenArmorDuration = 20f;
    private bool _canRegenArmor = false;
    private Coroutine _regenArmorTickCoroutine;
    public float RegenArmorAmount { get => _regenArmorAmount; set => _regenArmorAmount = value; }

    private void OnEnable()
    {
        //These are called to update the starting values of each bar.
        if (_healthChannel != null)
            _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
        if (_armorChannel != null)
            _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    public void HealFullHealthAndArmor()
    {
        HealArmor(_healthSystem.MaxArmor);
        HealHealth(_healthSystem.MaxHealth);
    }

    public void ResetHealth()
    {
        _healthSystem.ResetSystem();
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// MAX HEALTH INCREASES
    /// </summary>
    public void IncreaseMaxHealth(float amount)
    {
        _healthSystem.MaxHealth += amount;
        _healthSystem.HealHealth(amount);
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void IncreaseMaxArmor(float amount)
    {
        _healthSystem.MaxArmor += amount;
        _healthSystem.HealArmor(amount);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// HEALTH DAMAGE AND HEAL
    /// </summary>
    public void Damage(float damage)
    {
        float before = CurrentHealth + CurrentArmor;
        _healthSystem.Damage(damage);
        if (CurrentHealth + CurrentArmor < before)
        {
            _damageScreenFlash.Flash();
            if (Player.Instance != null)
                ControllerRumble.Play(Player.Instance.InputTranslator, 0.8f, 0.6f, 0.22f);
        }

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(damage, Player.Instance.transform.position, Color.red);

        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor); 
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealHealth(float amount)
    {
        //Debug.Log($"Healing health by amount: ${amount}");
        //Debug.Log($"invoking current health as: {_healthSystem.CurrentHealth} and max health as: {_healthSystem.MaxHealth}");

        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, Player.Instance.transform.position, Color.green);

        _healthSystem.HealHealth(amount);
        _healthChannel.Invoke(_healthSystem.CurrentHealth, _healthSystem.MaxHealth);
    }
    public void HealArmor(float amount)
    {
        //Debug.Log($"Healing armor by amount: ${amount}");
        //Debug.Log($"invoking current armor as: {_healthSystem.CurrentArmor} and max armor as: {_healthSystem.MaxArmor}");
        if (DamageLabelManager.Instance != null && DamageLabelManager.Instance.isActiveAndEnabled)
            DamageLabelManager.Instance.SpawnPopup(amount, Player.Instance.transform.position, Color.blue);

        _healthSystem.HealArmor(amount);
        _armorChannel.Invoke(_healthSystem.CurrentArmor, _healthSystem.MaxArmor);
    }

    /// <summary>
    /// HEALTH REGEN
    /// </summary>
    public void EnableHealthRegen()
    {
        if (_canRegenHealth) return;
        _canRegenHealth = true;
        _regenHealthTickCoroutine = StartCoroutine(RegenTickRate());
        StartCoroutine(DisableHealthRegenTimer());
    }
    private IEnumerator DisableHealthRegenTimer()
    {
        yield return new WaitForSeconds(_regenHealthDuration);
        _canRegenHealth = false;
        StopCoroutine(_regenHealthTickCoroutine);
    }
    private IEnumerator RegenTickRate()
    {
        while (_canRegenHealth)
        {
            yield return new WaitForSeconds(_regenHealthTickTime);

            if (_canRegenHealth && CurrentHealth < MaxHealth)
            {
                HealHealth(_regenHealthAmount);
            }   
        }
    }

    /// <summary>
    /// SHIELD REGEN
    /// </summary>
    public void EnableArmorRegen()
    {
        if (_canRegenArmor) return;
        _canRegenArmor = true;
        _regenArmorTickCoroutine = StartCoroutine(RegenArmorTickRate());
        StartCoroutine(DisableArmorRegenTimer());
    }
    private IEnumerator DisableArmorRegenTimer()
    {
        yield return new WaitForSeconds(_regenArmorDuration);
        _canRegenArmor = false;
        StopCoroutine(_regenArmorTickCoroutine);
    }
    private IEnumerator RegenArmorTickRate()
    {
        while (_canRegenArmor)
        {
            yield return new WaitForSeconds(_regenArmorTickTime);

            if (_canRegenArmor && CurrentArmor < MaxArmor)
            {
                HealArmor(_regenArmorAmount);
            }
        }
    }
    public HealthSystem GetPlayerHealthSystemRef()
    {
        return _healthSystem;
    }
}

// A separate UI overlay keeps damage feedback independent of the beat materials.
internal sealed class PlayerDamageScreenFlash : MonoBehaviour
{
    private const float Duration = 0.35f;
    private Canvas _canvas;
    private RawImage _image;
    private Texture2D _texture;
    private float _remaining;

    private void Awake()
    {
        var overlay = new GameObject("Damage Screen Flash", typeof(RectTransform), typeof(Canvas));
        overlay.transform.SetParent(transform, false);
        _canvas = overlay.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 32767;

        var imageObject = new GameObject("Red Edges", typeof(RectTransform), typeof(RawImage));
        imageObject.transform.SetParent(overlay.transform, false);
        _image = imageObject.GetComponent<RawImage>();
        _image.raycastTarget = false;
        var rect = _image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        const int size = 128;
        _texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        _texture.name = "Damage Edge Gradient";
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.filterMode = FilterMode.Bilinear;
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float horizontal = Mathf.Abs(2f * x / (size - 1) - 1f);
                float vertical = Mathf.Abs(2f * y / (size - 1) - 1f);
                float edge = Mathf.Max(horizontal, vertical);
                float alpha = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.94f, 1f, edge));
                pixels[y * size + x] = new Color(1f, 0f, 0f, alpha);
            }
        }
        _texture.SetPixels(pixels);
        _texture.Apply(false, true);
        _image.texture = _texture;
        _canvas.enabled = false;
    }

    public void Flash()
    {
        _remaining = Duration;
        _image.color = new Color(1f, 1f, 1f, 0.245f);
        _canvas.enabled = true;
    }

    private void Update()
    {
        if (_remaining <= 0f) return;
        _remaining = Mathf.Max(0f, _remaining - Time.unscaledDeltaTime);
        float strength = _remaining / Duration;
        _image.color = new Color(1f, 1f, 1f, 0.245f * strength * strength);
        _canvas.enabled = _remaining > 0f;
    }

    private void OnDisable()
    {
        _remaining = 0f;
        if (_canvas != null) _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        if (_texture != null) Destroy(_texture);
        if (_canvas != null) Destroy(_canvas.gameObject);
    }
}
