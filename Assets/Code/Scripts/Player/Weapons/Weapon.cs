using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchosCry.Combo;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Animator _attackAnimator;
    [SerializeField] private WeaponCollider _weaponCollider;
    [SerializeField] private ComboWeaponData _comboWeaponData;
    
    private AttackData _currentAttackData;
    private ComboTree _comboTree;

    private RuntimeAnimatorController _defaultAnimatorController;
    private string _animationClipName;

    public bool IsAttackEnded { get; private set; }
    public WeaponCollider Collider { get { return _weaponCollider; } }

    public void PrimaryAction(Stats multipliers)
    {
        SetChildrenActive(true);
        _weaponCollider.ClearColliderList();
        IsAttackEnded = false;
        
        _currentAttackData = _comboTree.ProcessPrimaryAction().AttackData;

        EchosCry.Sound.PlaySFX(_currentAttackData.AttackSound, transform, 0);

        Attack(multipliers);
    }
    public void SecondaryAction(Stats multipliers) 
    {
        SetChildrenActive(true);
        _weaponCollider.ClearColliderList();
        IsAttackEnded = false;
        
        _currentAttackData = _comboTree.ProcessSecondaryAction().AttackData;

        EchosCry.Sound.PlaySFX(_currentAttackData.AttackSound, transform, 0);

        Attack(multipliers);
    }
    public void AddEffect(StateName index, EffectData effect)
    {
        _comboTree.AddEffect(index, effect);
    }
    public void ResetEffects()
    {
        _comboTree.ResetEffects();
    }

    private void Attack(Stats multipliers)
    {
        StopAllCoroutines(); //Stop Coroutines, specifically ComboResetTimer

        AttackInfo attack = new AttackInfo.Builder()
            .SetDamage(_currentAttackData.BaseDamage * multipliers.DamageMultiplier)
            .SetForce(_currentAttackData.BaseForce * multipliers.KnockbackMultiplier)
            .SetForceMode(ForceMode.Impulse)
            .SetHitQuality(TempoConductor.Instance.CurrentHitQuality)
            .SetOrigin(Player.Instance.transform)
            .SetEffects(_comboTree.GetCurrentState().Effects.ToArray())
            .Build();

        _weaponCollider.UpdateAttack(attack);
        
        //Setup and play animations associated with the attack data
        AnimatorOverrideController controller = new AnimatorOverrideController(_attackAnimator.runtimeAnimatorController);
        controller[_animationClipName] = _currentAttackData.AnimationClip;
        _attackAnimator.runtimeAnimatorController = controller;
        _attackAnimator.Play("Attack");
        
        //Begin coroutine that will measure the animation length and then reset weapon
        StartCoroutine(AttackLengthCoroutine(_currentAttackData.AnimationClip.length));
    }
    private void Awake()
    {
        _defaultAnimatorController = _attackAnimator.runtimeAnimatorController;
        _animationClipName = _attackAnimator.runtimeAnimatorController.animationClips[0].name;
        
        SetChildrenActive(false);
        
        _comboTree = new();
        _comboTree.InitTreeAttackData(_comboWeaponData.AttackData);
    }
    private void AttackEnded()
    {
        _attackAnimator.runtimeAnimatorController = _defaultAnimatorController;
        IsAttackEnded = true;
       
        StartCoroutine(_comboTree.ComboResetTimer(_comboWeaponData.ComboResetTime));
        
        SetChildrenActive(false);
    }
    private void SetChildrenActive(bool active)
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    private IEnumerator AttackLengthCoroutine(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        AttackEnded();
    }
}