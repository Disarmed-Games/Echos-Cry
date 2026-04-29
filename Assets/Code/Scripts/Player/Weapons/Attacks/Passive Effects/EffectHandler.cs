using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EchosCry
{
    //public enum Effects
    //{
    //    None, Bleed, MarkForDeath, Critical, Flame
    //}
    public enum EffectTier
    {
        One, Two, Three
    }
}

public class EffectHandler : MonoBehaviour
{
    class EffectNode
    {
        public Coroutine coroutine;
        public int stacks;
        public EffectNode(Coroutine coroutine, int stacks)
        {
            this.coroutine = coroutine;
            this.stacks = stacks;
        }
    }

    private readonly Dictionary<string, EffectNode> _activeEffectData = new();
    private readonly HashSet<string> _activeEffects = new();

    private void OnDisable()
    {
        _activeEffects.Clear();
        _activeEffectData.Clear();
    }

    public void ApplyEffect(EffectData effect, Enemy enemy)
    {
        string effectName = effect.EffectName;

        if (_activeEffects.Contains(effectName)) //Check if effect active
        {
            int newStack = _activeEffectData[effectName].stacks + 1; 
            if (newStack > effect.MaxStacks) return; //If new stack count greater than max count, return

            _activeEffectData[effectName].stacks = newStack;

            return;
        }

        _activeEffects.Add(effectName); //Add effect to active effects

        StartCoroutine(EndRoutineEffect(effect)); //Start coroutine for effect duration

        _activeEffectData.Add(effectName, new EffectNode(null, 1)); //Add to activeEffectData
        if(effect.IsEffectUseOneTime) 
        {
            UseEffect(effect, enemy, 1); //Use once if only one time use
        }
        else
        {
            _activeEffectData[effectName].coroutine = StartCoroutine(RoutineEffect(effect, enemy)); //Else start coroutine
        }
    }

    public void RemovePassiveEffect(EffectData effect)
    {
        string effectEnum = effect.EffectName;

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

    private IEnumerator RoutineEffect(EffectData effect, Enemy enemy)
    {
        while (_activeEffects.Contains(effect.EffectName))
        {
            UseEffect(effect, enemy, _activeEffectData[effect.EffectName].stacks);
            yield return new WaitForSeconds(effect.EffectUseInterval);
        }
    }

    private void UseEffect(EffectData effectData, Enemy enemy, int stackCount)
    {
        //Use every effect attributed to effectData
        foreach (Effect effect in effectData.Effects)
        {
            effect.Use(enemy, this, stackCount, effectData.EffectDuration);
        }
    }
}