using AudioSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour
{
    [SerializeField] private soundEffect tapSFX;
    [SerializeField] private InputTranslator _inputTranslator;
    [SerializeField] private TextMeshProUGUI _resultsText;
    [SerializeField] private Button _continueButton;
    private float _currentBeatProgress = 0;
    private int _hitCount = 0;
    private int _maxHitCount = 5;
    private float _overallAccuracy = 0;
    private List<float> _accuracyList = new List<float>();

    public static float HitAccuracy;

    private void OnEnable()
    {
        _inputTranslator.OnPrimaryActionEvent += CheckHitAccuracy;
        DisplayResults();
    }
    private void OnDisable()
    {
        _inputTranslator.OnPrimaryActionEvent -= CheckHitAccuracy;
    }

    private void CheckHitAccuracy(bool isPressed)
    {
        if (!isPressed
            || _hitCount >= _maxHitCount
            || EventSystem.current.IsPointerOverGameObject()) return;

        EchosCry.Sound.PlaySFX(tapSFX, this.transform, 0);

        //Debug.Log(_currentBeatProgress);
        float distanceFromBeat = Mathf.Min(_currentBeatProgress, 1f - _currentBeatProgress);
        _accuracyList.Add(distanceFromBeat);
        _hitCount++;

        _continueButton.interactable = (_hitCount >= _maxHitCount);

        CalculateOverallAccuracy();
    }
    private void CalculateOverallAccuracy()
    {
        //Calculate Overall Accuracy
        float total = 0;
        foreach (var accuracy in _accuracyList)
        {
            total += accuracy;
        }

        float averageDistance = total / _accuracyList.Count;
        _overallAccuracy = 1f - (averageDistance * 2f);
        DisplayResults();
    }

    private void DisplayResults()
    {
        _resultsText.text = $"Results:\nBeat Hit: {_hitCount} | Accuracy: {(_overallAccuracy * 100f):F1}%";
    }
    private void Update()
    {
        _currentBeatProgress = BeatManager.Instance.BeatProgress;
    }
    public void ContinueButton()
    {
        if (_hitCount == _maxHitCount)
        {
            HitAccuracy = _overallAccuracy;
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Cutscene);
        }
    }
    public void RetryButton()
    {
        _accuracyList.Clear();
        _hitCount = 0;
        _overallAccuracy = 0;
        _continueButton.interactable = false;
        DisplayResults();
    }
}
