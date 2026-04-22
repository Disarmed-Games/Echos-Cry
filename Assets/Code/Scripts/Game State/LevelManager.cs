using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Level
{
    public LevelManager.LevelName name;
    public bool locked;
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    public static event Action<LevelName> LevelUnlockedEvent;

    public enum LevelName
    {
        LEVEL_ONE,
        LEVEL_TWO,
        LEVEL_THREE,
        LEVEL_FOUR,
        LEVEL_FIVE,
    }

    [SerializeField] private List<Level> LevelList = new();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        GameManager.OnGameOverEvent += LockAllLevels;
    }
    private void OnDestroy()
    {
        GameManager.OnGameOverEvent -= LockAllLevels;
    }

    public void UnlockLevel(LevelName levelName)
    {
        int index = (int)levelName;
        if (index >= LevelList.Count) { return; }

        for (int i = 0; i < LevelList.Count; i++)
        {
            if (LevelList[i].name == levelName)
            {
                Level newLevel = LevelList[i];
                newLevel.locked = false;
                LevelList[i] = newLevel;
                break;
            }
        }

        LevelUnlockedEvent?.Invoke(levelName);
    }

    public void LockAllLevels()
    {
        for (int i = 1; i < LevelList.Count; i++) //Skip locking the first level!
        {
            Level newLevel = LevelList[i];
            newLevel.locked = true;
            LevelList[i] = newLevel;
            LevelUnlockedEvent?.Invoke(LevelList[i].name);
        }
    }

    public bool GetLevelLockedStatus(LevelName levelName)
    {
        for (int i = 0; i < LevelList.Count; i++)
        {
            if (LevelList[i].name == levelName)
            {
                return LevelList[i].locked;
            }
        }
        return false;
    }
}
