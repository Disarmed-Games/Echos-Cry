using System;
using System.Collections;
using UnityEngine;
using EchosCry.Combo;
using Ink.Parsed;
using System.Collections.Generic;

namespace EchosCry.Combo
{
    public class ComboState
    {
        public ComboState NextLightAttack = null;
        public ComboState NextHeavyAttack = null;
        public AttackData AttackData = null;
        public List<EffectData> Effects = new();
    }

    public enum StateName
    {
        Start = -1,
        Light1, Light2, Light3, Light4, Light5,
        Heavy1, Heavy2, Heavy3
    }

}

public class ComboTree
{
    //Tree root
    private ComboState   _startState;

    //Current state
    private ComboState   _currentState;

    //Array that holds states
    private ComboState[] _comboStates;

    public ComboTree()
    {
        InitTree();
    }

    public void AddEffect(StateName index, EffectData effect)
    {
        _comboStates[(int)index].Effects.Add(effect);
    }
    public void ResetEffects()
    {
        foreach(ComboState state in _comboStates)
        {
            state.Effects.Clear();
        }
    }
    public ComboState GetCurrentState()
    {
        return _currentState;
    }
    public ComboState ProcessPrimaryAction()
    {
        if (_currentState == null) return null;
        if (_currentState.NextLightAttack == null) _currentState = _startState.NextLightAttack;
        else _currentState = _currentState.NextLightAttack;
        return _currentState;
    }
    public ComboState ProcessSecondaryAction()
    {
        if (_currentState == null) return null;
        if (_currentState.NextHeavyAttack == null) _currentState = _startState.NextHeavyAttack;
        else _currentState = _currentState.NextHeavyAttack;
        return _currentState;
    }

    public IEnumerator ComboResetTimer(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        _currentState = _startState;
    }

    private void InitTree()
    {
        _comboStates = new ComboState[8];
        for(int i = 0; i < _comboStates.Length; i++)
        {
            _comboStates[i] = new();
        }

        _comboStates[(int)StateName.Light1].NextLightAttack = _comboStates[(int)StateName.Light2];

        _comboStates[(int)StateName.Light2].NextLightAttack = _comboStates[(int)StateName.Light3];
        _comboStates[(int)StateName.Light2].NextHeavyAttack = _comboStates[(int)StateName.Heavy3];

        _comboStates[(int)StateName.Heavy1].NextHeavyAttack = _comboStates[(int)StateName.Heavy2];
        _comboStates[(int)StateName.Heavy1].NextLightAttack = _comboStates[(int)StateName.Light4];

        _comboStates[(int)StateName.Light4].NextLightAttack = _comboStates[(int)StateName.Light5];
        
        _startState = new()
        {
            NextLightAttack = _comboStates[(int)StateName.Light1],
            NextHeavyAttack = _comboStates[(int)StateName.Heavy1],
            AttackData = null
        };

        _currentState = _startState;
    }
    public void InitTreeAttackData(AttackData[] attackData)
    {
        if (_comboStates == null) InitTree();
        
        if(_comboStates.Length != attackData.Length)
        {
            Debug.LogError("Attack Data array is not a valid size for the Combo States Array");
            return;
        }
        
        for(int i = 0; i < _comboStates.Length; i++)
        {
            _comboStates[i].AttackData = attackData[i];
        }
    }
}
