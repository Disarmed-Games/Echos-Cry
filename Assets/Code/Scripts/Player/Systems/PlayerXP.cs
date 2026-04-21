using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    private int _currentLevel = 0;
    private float _currentXPAmount;
    private float _goalXPAmount = 10f;
    [SerializeField] private XPCalculationStrategy _newXPGoalCalculation;
    [SerializeField] private FloatFloatIntEventChannel _updateXPChannel;
    [SerializeField] private IntEventChannel _levelUpChannel;

    public int CurrentLevel { get => _currentLevel; }
    public float CurrentXPAmount { get => _currentXPAmount; }
    public float GoalXPAmount { get => _goalXPAmount; }

    private void Start()
    {
        _goalXPAmount = _newXPGoalCalculation.Execute(this);
        if (_updateXPChannel != null) _updateXPChannel.Invoke(_currentXPAmount, _goalXPAmount, _currentLevel);
    }

    public void IncreaseXP(float xp)
    {
        _currentXPAmount += xp;

        while (_currentXPAmount >= _goalXPAmount)
        {
            _currentLevel++;

            if (_levelUpChannel != null)
                _levelUpChannel.Invoke(_currentLevel);

            _currentXPAmount -= _goalXPAmount;

            if (_newXPGoalCalculation != null)
                _goalXPAmount = _newXPGoalCalculation.Execute(this);
            else
                _goalXPAmount = _goalXPAmount + _currentLevel * 1.5f;
        }

        if (_updateXPChannel != null)
            _updateXPChannel.Invoke(_currentXPAmount, _goalXPAmount, _currentLevel);
    }
    public void ResetXP()
    {
        _currentXPAmount = 0;
    }
}