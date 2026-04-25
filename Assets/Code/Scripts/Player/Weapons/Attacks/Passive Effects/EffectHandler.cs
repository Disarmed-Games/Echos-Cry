using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EchosCry
{
    public enum Effects
    {
        Bleed, MarkForDeath, Critical
    }
}

public class EffectHandler : MonoBehaviour
{
    struct EffectNode
    {
        public Coroutine coroutine;
        public int stacks;
        public EffectNode(Coroutine coroutine, int stacks)
        {
            this.coroutine = coroutine;
            this.stacks = stacks;
        }
    }

    [SerializeField] private Enemy enemyReference;

    private readonly Dictionary<EchosCry.Effects, EffectNode> _activePassiveEffects = new();

    private void OnDisable()
    {
        _activePassiveEffects.Clear();
    }

    public void ApplyEffect(EffectData effect)
    {
        EchosCry.Effects effectType = effect.EffectEnum;
        Debug.Log(effectType);

        if (_activePassiveEffects.ContainsKey(effectType)) //Check if effect active
        {
            int newStack = _activePassiveEffects[effectType].stacks + 1; 
            if (newStack > _activePassiveEffects[effectType].stacks) return; //If new stack count greater than max count, return

            Coroutine newRoutine = null;
            if (_activePassiveEffects[effectType].coroutine != null)
            {
                StopCoroutine(_activePassiveEffects[effectType].coroutine); //Stop prior routine interval if has it
                newRoutine = StartCoroutine(RoutineEffect(effect)); //Restart coroutine
            }

            _activePassiveEffects.Remove(effectType);
            _activePassiveEffects.Add(effectType, new EffectNode(newRoutine, newStack));
            return;
        }

        _activePassiveEffects.Add(effectType, new EffectNode(null, 1)); //Add passive effect so it is registered to the dictionary to be checked
        StartCoroutine(EndRoutineEffect(effect));

        Coroutine routine = null;
        if(effect.IsEffectUseOneTime) 
        {
            UseEffect(effect);
        }
        else
        {
            routine = StartCoroutine(RoutineEffect(effect));
        }

        _activePassiveEffects[effectType] = new EffectNode(routine, 1);
    }

    public void RemovePassiveEffect(EffectData effect)
    {
        EchosCry.Effects type = effect.EffectEnum;

        if (_activePassiveEffects[type].coroutine != null)
        {
            StopCoroutine(_activePassiveEffects[type].coroutine);
        }
        _activePassiveEffects.Remove(type);
    }

    private IEnumerator EndRoutineEffect(EffectData effect)
    {
        yield return new WaitForSeconds(effect.EffectDuration);
        RemovePassiveEffect(effect);
    }

    private IEnumerator RoutineEffect(EffectData effect)
    {
        while (_activePassiveEffects.ContainsKey(effect.EffectEnum))
        {
            yield return new WaitForSeconds(effect.EffectUseInterval);
            
            UseEffect(effect);
        }
    }

    public void UseEffect(EffectData effectData)
    {
        foreach (Effect effect in effectData.Effects)
        {
            Debug.Log("Using effect");
            effect.Use(enemyReference, this);
        }
    }
}