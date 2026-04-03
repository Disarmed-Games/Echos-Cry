using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Animator _attackAnimator;
    [SerializeField] protected WeaponCollider _weaponCollider;
    [SerializeField] protected PlayerAttackDamage _playerAttackDamage;

    protected RuntimeAnimatorController _defaultAnimatorController;

    [HideInInspector]
    public AttackData _currentAttackData;
    public bool IsAttackEnded { get; private set; }

    public struct ColliderInfo
    {
        public Collider collider;
        public TempoConductor.HitQuality hit;
    }
    private List<ColliderInfo> _hitColliders;
    public List<ColliderInfo> HitColliders { get => _hitColliders; }

    protected float baseDamageMultiplier = 1f;

    public void AddColliderToList(Collider collider, TempoConductor.HitQuality hit)
    {
        if (_hitColliders == null || collider == null) return;
        _hitColliders.Add(new ColliderInfo { collider = collider, hit = hit });
    }
    public void ClearColliderList()
    {
        _hitColliders.Clear();
    }

    protected virtual void OnAwake() { } 
    protected virtual void OnPrimaryAction() { }
    protected virtual void OnSecondaryAction() { }
    protected virtual void OnAttackEnded() { }
    protected abstract void Attack();

    private void Awake()
    {
        _defaultAnimatorController = _attackAnimator.runtimeAnimatorController;
        _hitColliders = new();
        SetChildrenActive(false);
        OnAwake();
    }
    private void AttackEnded()
    {
        _attackAnimator.runtimeAnimatorController = _defaultAnimatorController;
        IsAttackEnded = true;
        OnAttackEnded();
        SetChildrenActive(false);
    }
    public void PrimaryAction()
    {
        SetChildrenActive(true);
        ClearColliderList();
        IsAttackEnded = false;
        OnPrimaryAction();
        Attack();
    }
    public void SecondaryAction() 
    {
        SetChildrenActive(true);
        ClearColliderList();
        IsAttackEnded = false;
        OnSecondaryAction();
        Attack();
    }

    private void SetChildrenActive(bool active)
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    protected IEnumerator AttackLengthCoroutine(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        AttackEnded();
    }
}