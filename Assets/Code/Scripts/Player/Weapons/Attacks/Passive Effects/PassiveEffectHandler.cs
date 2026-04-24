using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassiveEffectHandler : MonoBehaviour
{
    [SerializeField] private Enemy enemyReference;

    private HashSet<Type> _passiveEffectSet = new();
    private Dictionary<Type, Coroutine> _activeRoutines = new();
    
    public void UsePassiveEffect(PassiveEffect effect)
    {
        Type effectType = effect.GetType();

        if (!_passiveEffectSet.Add(effectType)) return; //Avoid duplicate effects. Can't use variants of the same effect at the same time, but can use different effects.

        ApplyEffect(effect);
    }

    public void RemovePassiveEffect(PassiveEffect effect)
    {
        Type type = effect.GetType();
        
        if (_activeRoutines.TryGetValue(type, out var routine))
        {
            StopCoroutine(routine);
            _activeRoutines.Remove(type);
        }

        _passiveEffectSet.Remove(type);
    }

    private void ApplyEffect(PassiveEffect effect)
    {
        StartCoroutine(EndRoutineEffect(effect));

        if (effect.isEffectOneTime)
        {
            UseEffect(effect);
        }
        else
        {
            Coroutine routine = StartCoroutine(RoutineEffect(effect));
            _activeRoutines[effect.GetType()] = routine;
        }
    }

    private IEnumerator EndRoutineEffect(PassiveEffect effect)
    {
        yield return new WaitForSeconds(effect.effectDuration);
        RemovePassiveEffect(effect);
    }

    private IEnumerator RoutineEffect(PassiveEffect effect)
    {
        while (_passiveEffectSet.Contains(effect.GetType()))
        {
            yield return new WaitForSeconds(effect.effectUseInterval);
            UseEffect(effect);
        }
    }

    //ISSUE: Instead of directly referencing the health systems through the enemy, use NPC Damageable
    //Also seems that the damage multiplier stuff should happen in the NPC Damageable script rather than the Health System
    public void UseEffect(PassiveEffect effect)
    {
        effect.Use(enemyReference);
    }
}