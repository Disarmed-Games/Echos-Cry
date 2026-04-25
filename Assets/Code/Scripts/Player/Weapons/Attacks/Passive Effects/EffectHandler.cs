using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private Dictionary<Type, EffectNode> _activePassiveEffects = new();

    private void OnDisable()
    {
        _activePassiveEffects.Clear();
    }

    public void ApplyEffect(EffectData effect)
    {
        Type effectType = effect.GetType();

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

        _activePassiveEffects.Add(effectType, new EffectNode(routine, 1));
        
    }

    public void RemovePassiveEffect(EffectData effect)
    {
        Type type = effect.GetType();

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
        while (_activePassiveEffects.ContainsKey(effect.GetType()))
        {
            yield return new WaitForSeconds(effect.EffectUseInterval);
            UseEffect(effect);
        }
    }

    public void UseEffect(EffectData effectData)
    {
        foreach (Effect effect in effectData.Effects)
        {
            effect.Use(enemyReference, this);
        }
    }
}