using Mono.Cecil.Cil;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponInventory;
    [SerializeField] private GameObject _dashWeapon;

    private Weapon _currentlyEquippedWeapon;
    private Weapon _previouslyEquippedWeapon;
    public Weapon CurrentlyEquippedWeapon
    {
        get => _currentlyEquippedWeapon;
        set => _currentlyEquippedWeapon = value; 
    }
    public bool HasWeapon => _currentlyEquippedWeapon != null;

    public bool DidWeaponHit => _currentlyEquippedWeapon.HitColliders.Count > 0;

    private void Awake()
    {
        if (_weaponInventory.Length > 0)
        {
            _currentlyEquippedWeapon = _weaponInventory[0].GetComponent<Weapon>();
            ActivateWeapon();
        }
    }

    private void ActivateWeapon()
    {
        foreach (var weapon in _weaponInventory)
        {
            weapon.SetActive(weapon.GetComponent<Weapon>() == _currentlyEquippedWeapon);
        }
    }

    private void DeactivateAllWeapons()
    {
        foreach (var weapon in _weaponInventory)
        {
            weapon.SetActive(false);
        }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (_weaponInventory.Length == 0) return;
        if (!_currentlyEquippedWeapon.IsAttackEnded) return;

        //Logic for choosing next weapon in array
        //int currentIndex = System.Array.IndexOf(_weaponInventory, _currentlyEquippedWeapon.gameObject);
        //int nextIndex = (currentIndex + 1) % _weaponInventory.Length;

        _currentlyEquippedWeapon = _weaponInventory[weaponIndex].GetComponent<Weapon>();
        ActivateWeapon();
    }

    public void PrimaryAction()
    {
        if (_currentlyEquippedWeapon == null) return;

        SwitchWeapon(0);

        _currentlyEquippedWeapon.PrimaryAction();
    }
    public void SecondaryAction() 
    {
        if (_currentlyEquippedWeapon == null) return;

        if (BeatManager.Instance.BeatInMeasure == 3)
        {
            SwitchWeapon(1);
            _currentlyEquippedWeapon.PrimaryAction();
        }
        else if (BeatManager.Instance.BeatInMeasure == 1)
        {
            SwitchWeapon(1);
            _currentlyEquippedWeapon.SecondaryAction();
        }
        else
        {
            SwitchWeapon(0);
            _currentlyEquippedWeapon.SecondaryAction();
        }
    }
    public void DashAction()
    {
        if (_currentlyEquippedWeapon == null) return;
        _previouslyEquippedWeapon = _currentlyEquippedWeapon;
        DeactivateAllWeapons();
        _currentlyEquippedWeapon = _dashWeapon.GetComponent<Weapon>();
        _currentlyEquippedWeapon.PrimaryAction();
    }
    public void ResetPreviousWeapon()
    {
        _currentlyEquippedWeapon = _previouslyEquippedWeapon;
        ActivateWeapon();
    }

    public bool IsActionEnded()
    {
        return _currentlyEquippedWeapon.IsAttackEnded;
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
