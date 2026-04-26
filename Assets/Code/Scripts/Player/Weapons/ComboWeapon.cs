using System.Collections.Generic;
using UnityEngine;

//public class ComboWeapon : Weapon
//{
//    private ComboTree _comboTree;

//    [SerializeField] private ComboWeaponData _comboWeaponData;
//    private string _clipName;

//    protected override void Attack()
//    {
//        //Stop Coroutines, specifically ComboResetTimer
//        StopAllCoroutines();
//        //Update the damage the weapon collider will use based on attack data
//        AttackInfo attack = new AttackInfo.Builder()
//            .SetDamage(_currentAttackData.BaseDamage * _playerAttackDamage.BaseDamageMultiplier)
//            .SetForce(_currentAttackData.BaseForce * _playerKnockback.BaseKnockbackMultiplier)
//            .SetForceMode(ForceMode.Impulse)
//            .SetHitQuality(TempoConductor.Instance.CurrentHitQuality)
//            .SetOrigin(Player.Instance.transform)
//            .Build();
//        _weaponCollider.UpdateAttack(attack);
//        //Setup and play animations associated with the attack data
//        AnimatorOverrideController controller = new AnimatorOverrideController(_attackAnimator.runtimeAnimatorController);
//        controller[_clipName] = _currentAttackData.AnimationClip;
//        _attackAnimator.runtimeAnimatorController = controller;
//        _attackAnimator.Play("Attack");
//        //Begin coroutine that will measure the animation length and then reset weapon

//        StartCoroutine(AttackLengthCoroutine(_currentAttackData.AnimationClip.length));
//    }

//    protected override void OnAwake()
//    {
//        _comboTree = new();
//        _comboTree.InitTreeAttackData(_comboWeaponData.AttackData);

//        _clipName = _attackAnimator.runtimeAnimatorController.animationClips[0].name;
//    }

//    protected override void OnPrimaryAction()
//    {
//        _currentAttackData = _comboTree.ProcessPrimaryAction().AttackData;
        
//        //Use SFX manager to setup attack sound
//        SoundEffectManager.Instance.Builder
//            .SetSound(_currentAttackData.AttackSound)
//            .SetSoundPosition(transform.position)
//            .ValidateAndPlaySound();
//    }

//    protected override void OnSecondaryAction()
//    {
//        _currentAttackData = _comboTree.ProcessSecondaryAction().AttackData;

//        //Use SFX manager to setup attack sound
//        SoundEffectManager.Instance.Builder
//            .SetSound(_currentAttackData.AttackSound)
//            .SetSoundPosition(transform.position)
//            .ValidateAndPlaySound();
//    }

//    protected override void OnAttackEnded()
//    {
//        StartCoroutine(_comboTree.ComboResetTimer(_comboWeaponData.ComboResetTime));
//    }
//}