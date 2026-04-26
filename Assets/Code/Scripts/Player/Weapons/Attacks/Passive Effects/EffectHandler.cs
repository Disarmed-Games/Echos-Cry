using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EchosCry
{
    public enum Effects
    {
        None, Bleed, MarkForDeath, Critical, Flame
    }
    public enum EffectTier
    {
        One, Two, Three
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
            if (newStack > effect.MaxStacks) return; //If new stack count greater than max count, return

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

        _activeEffects.Add(effectEnum); //Add effect to active effects

        StartCoroutine(EndRoutineEffect(effect)); //Start coroutine for effect duration

        Coroutine routine = null;
        if(effect.IsEffectUseOneTime) 
        {
            UseEffect(effect, 1); //Use once if only one time use
        }
        else
        {
            routine = StartCoroutine(RoutineEffect(effect)); //Else start coroutine
        }

        _activeEffectData.Add(effectEnum, new EffectNode(routine, 1)); //Add to activeEffectData
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
            
            UseEffect(effect, _activeEffectData[effect.EffectEnum].stacks);
        }
    }

    public void UseEffect(EffectData effectData, int stackCount)
    {
        //Use every effect attributed to effectData
        foreach (Effect effect in effectData.Effects)
        {
            effect.Use(enemyReference, this, stackCount);
        }
    }
}