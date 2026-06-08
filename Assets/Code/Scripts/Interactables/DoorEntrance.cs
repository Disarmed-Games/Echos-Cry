using UnityEngine;
using UnityEngine.InputSystem;

public class DoorEntrance : DoorManager
{
    [SerializeField] private LevelManager.LevelName levelReferenceName;
    [SerializeField] private GameObject _lockObject;
    [SerializeField] private GameObject _arrowDecal;

    protected void Start()
    {
        isLocked = LevelManager.Instance.GetLevelLockedStatus(levelReferenceName);
    }

    private void Update()
    {
        _lockObject.SetActive(isLocked);
        _arrowDecal.SetActive(!isLocked);
    }
}
