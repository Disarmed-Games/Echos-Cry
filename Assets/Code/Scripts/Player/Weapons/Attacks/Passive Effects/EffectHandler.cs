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

    private readonly Dictionary<string, EffectNode> _activeEffectData = new();
    private readonly HashSet<string> _activeEffects = new();

    private void OnDisable()
    {
        _activeEffects.Clear();
        _activeEffectData.Clear();
    }

    public void ApplyEffect(EffectData effect, Enemy enemy)
    {
        string effectEnum = effect.EffectName;

        if (_activeEffects.Contains(effectEnum)) //Check if effect active
        {
            int newStack = _activeEffectData[effectEnum].stacks + 1; 
            if (newStack > effect.MaxStacks) return; //If new stack count greater than max count, return

            _activeEffectData.Remove(effectEnum);

            _activeEffectData.Add(effectEnum, new EffectNode(_activeEffectData[effectEnum].coroutine, newStack));

            return;
        }

        _activeEffects.Add(effectEnum); //Add effect to active effects

        foreach (Effect e in effect.Effects)
        {
            e.Apply(enemy, this);
        }

        StartCoroutine(EndRoutineEffect(effect, enemy)); //Start coroutine for effect duration

        Coroutine routine = null;
        if(effect.IsEffectUseOneTime) 
        {
            UseEffect(effect, enemy, 1); //Use once if only one time use
        }
        else
        {
            routine = StartCoroutine(RoutineEffect(effect, enemy)); //Else start coroutine
        }

        _activeEffectData.Add(effectEnum, new EffectNode(routine, 1)); //Add to activeEffectData
    }

    public void RemovePassiveEffect(EffectData effect, Enemy enemy)
    {
        string effectEnum = effect.EffectName;

        if (_activeEffectData[effectEnum].coroutine != null)
        {
            StopCoroutine(_activeEffectData[effectEnum].coroutine);
        }
        _activeEffects.Remove(effectEnum);
        _activeEffectData.Remove(effectEnum);

        foreach (Effect e in effect.Effects)
        {
            e.Remove(enemy, this);
        }
    }

    private IEnumerator EndRoutineEffect(EffectData effect, Enemy enemy)
    {
        yield return new WaitForSeconds(effect.EffectDuration);
        RemovePassiveEffect(effect, enemy);
    }

    private IEnumerator RoutineEffect(EffectData effect, Enemy enemy)
    {
        while (_activeEffects.Contains(effect.EffectName))
        {
            yield return new WaitForSeconds(effect.EffectUseInterval);
            
            UseEffect(effect, enemy, _activeEffectData[effect.EffectName].stacks);
        }
    }

    private void UseEffect(EffectData effectData, Enemy enemy, int stackCount)
    {
        //Use every effect attributed to effectData
        foreach (Effect effect in effectData.Effects)
        {
            effect.Use(enemy, this, stackCount);
        }
    }
}