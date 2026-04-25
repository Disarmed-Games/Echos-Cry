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

    private readonly Dictionary<EchosCry.Effects, EffectNode> _activeEffectData = new();
    private readonly HashSet<EchosCry.Effects> _activeEffects = new();

    private void OnDisable()
    {
        _activeEffects.Clear();
        _activeEffectData.Clear();
    }

    public void ApplyEffect(EffectData effect)
    {
        EchosCry.Effects effectEnum = effect.EffectEnum;

        if (_activeEffects.Contains(effectEnum)) //Check if effect active
        {
            int newStack = _activeEffectData[effectEnum].stacks + 1; 
            if (newStack > _activeEffectData[effectEnum].stacks) return; //If new stack count greater than max count, return

            Coroutine newRoutine = null;
            if (_activeEffectData[effectEnum].coroutine != null)
            {
                StopCoroutine(_activeEffectData[effectEnum].coroutine); //Stop prior routine interval if has it
                newRoutine = StartCoroutine(RoutineEffect(effect)); //Restart coroutine
            }

            _activeEffectData.Remove(effectEnum);
            _activeEffectData.Add(effectEnum, new EffectNode(newRoutine, newStack));
            return;
        }

        _activeEffects.Add(effectEnum); 

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

        _activeEffectData.Add(effectEnum, new EffectNode(routine, 1));
    }

    public void RemovePassiveEffect(EffectData effect)
    {
        EchosCry.Effects effectEnum = effect.EffectEnum;

        if (_activeEffectData[effectEnum].coroutine != null)
        {
            StopCoroutine(_activeEffectData[effectEnum].coroutine);
        }
        _activeEffects.Remove(effectEnum);
        _activeEffectData.Remove(effectEnum);
    }

    private IEnumerator EndRoutineEffect(EffectData effect)
    {
        yield return new WaitForSeconds(effect.EffectDuration);
        RemovePassiveEffect(effect);
    }

    private IEnumerator RoutineEffect(EffectData effect)
    {
        while (_activeEffects.Contains(effect.EffectEnum))
        {
            yield return new WaitForSeconds(effect.EffectUseInterval);
            
            UseEffect(effect);
        }
    }

    public void UseEffect(EffectData effectData)
    {
        //Use every effect attributed to effectData
        foreach (Effect effect in effectData.Effects)
        {
            effect.Use(enemyReference, this);
        }
    }
}