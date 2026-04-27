using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponInventory;

    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon specialWeapon;

    private Weapon _currentlyEquippedWeapon;

    public Weapon CurrentlyEquippedWeapon
    {
        get => _currentlyEquippedWeapon;
    }
    public bool HasWeapon => _currentlyEquippedWeapon != null;
    public bool DidWeaponHit => _currentlyEquippedWeapon.Collider.HitColliders.Count > 0;
    public bool IsActionEnded => _currentlyEquippedWeapon.IsAttackEnded;

    private void Awake()
    {
        _currentlyEquippedWeapon = primaryWeapon;
    }

    public void ActivateCurrentWeapon()
    {
        _currentlyEquippedWeapon.gameObject.SetActive(true);
    }

    public void DeactivateCurrentWeapon()
    {
        _currentlyEquippedWeapon.gameObject.SetActive(false);
    }

    public void SwitchToPrimary()
    {
        _currentlyEquippedWeapon = primaryWeapon;
    }
    public void SwitchToSpecial()
    {
        _currentlyEquippedWeapon = specialWeapon;
    }

    public void PrimaryAction(Stats stats)
    {
        _currentlyEquippedWeapon.PrimaryAction(stats);
    }
    public void SecondaryAction(Stats stats) 
    {
        _currentlyEquippedWeapon.SecondaryAction(stats);   
    }

    public void ProcessWeaponHits(PlayerComboMeter comboMeter)
    {
        int hitCount = _currentlyEquippedWeapon.Collider.HitColliders.Count;

        for (int i = 0; i < hitCount; i++)
        {
            comboMeter.AddToComboMeter(_currentlyEquippedWeapon.Collider.HitColliders[i].hitQuality);
        }
    }
    public void AddEffectPrimary(EchosCry.Combo.StateName index, EffectData data)
    {
        primaryWeapon.AddEffect(index, data);   
    }
    public void AddEffectSpecial(EchosCry.Combo.StateName index, EffectData data)
    {
        specialWeapon.AddEffect(index, data);
    }
    public void ResetEffects()
    {
        primaryWeapon.ResetEffects();
        specialWeapon.ResetEffects();
    }
}
