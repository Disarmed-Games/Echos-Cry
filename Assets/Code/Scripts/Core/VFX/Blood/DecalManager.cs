using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalManager : NonSpawnableSingleton<DecalManager>
{
    [SerializeField] GameObject _bloodPrefab;
    [SerializeField] float _timeBeforeFade = 10f;
    [SerializeField] float _fadeMultiplier = 3f;
    DecalPool _bloodPool;

    private void Awake()
    {
        _bloodPool = new DecalPool();
        _bloodPool.Initialize(_bloodPrefab, this, _timeBeforeFade, _fadeMultiplier);
    }

    public DecalProjector GetBloodDecal()
    {
        return _bloodPool.GetDecal();
    }
}
