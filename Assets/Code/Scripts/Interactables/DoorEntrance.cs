using UnityEngine;
using UnityEngine.InputSystem;

public class DoorEntrance : DoorManager
{
    [SerializeField] private LevelManager.LevelName levelReferenceName;
    [SerializeField] private GameObject _lockObject;

    protected void Start()
    {
        isLocked = LevelManager.Instance.GetLevelLockedStatus(levelReferenceName);
    }

    private void Update()
    {
        _lockObject.SetActive(isLocked);
    }
}
