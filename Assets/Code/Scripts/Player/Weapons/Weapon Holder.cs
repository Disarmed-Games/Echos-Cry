using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponInventory;
    [SerializeField] private GameObject _dashWeapon;

    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon specialWeapon;

    private Weapon _currentlyEquippedWeapon;

    public Weapon CurrentlyEquippedWeapon
    {
        get => _currentlyEquippedWeapon;
        set => _currentlyEquippedWeapon = value; 
    }
    public bool HasWeapon => _currentlyEquippedWeapon != null;
    public bool DidWeaponHit => _currentlyEquippedWeapon.HitColliders.Count > 0;
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

    public void PrimaryAction()
    {
        _currentlyEquippedWeapon.PrimaryAction();
    }
    public void SecondaryAction() 
    {
        _currentlyEquippedWeapon.SecondaryAction();   
    }

    public void ProcessWeaponHits(PlayerComboMeter comboMeter)
    {
        int hitCount = _currentlyEquippedWeapon.HitColliders.Count;

        for (int i = 0; i < hitCount; i++)
        {
            TempoConductor.HitQuality hitQuality = _currentlyEquippedWeapon.HitColliders[i].hit;
            comboMeter.AddToComboMeter(hitQuality);
        }
    }
}
