using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class DecalPool
{
    DecalManager _parent;
    float _timeBeforeFade;
    float _fadeMultiplier;
    GameObject _prefab;
    ObjectPool<DecalProjector> _decalPool;

    public DecalPool()
    {
        _decalPool = new ObjectPool<DecalProjector>
         (
             createFunc: OnCreateDecal,
             actionOnGet: OnGetDecal,
             actionOnRelease: OnReleaseDecal,
             actionOnDestroy: OnDestroyDecal,
             true,
             10,
             100
         );
    }
    public void Initialize(GameObject prefab, DecalManager parent, float timeBeforeFade, float fadeMulti)
    {
        _prefab = prefab;
        _parent = parent;
        _timeBeforeFade = timeBeforeFade;
        _fadeMultiplier = fadeMulti;
    }

    public DecalProjector GetDecal()
    {
        return _decalPool.Get();
    }
    public void ReleaseDecal(DecalProjector decal)
    {
        _decalPool.Release(decal);
    }

    private IEnumerator FadeAfterTime(DecalProjector decal)
    {
        yield return new WaitForSeconds(_timeBeforeFade);
        _parent.StartCoroutine(BeginFade(decal));
    }
    private IEnumerator BeginFade(DecalProjector decal)
    {
        while (decal.fadeFactor > 0)
        {
            decal.fadeFactor -= Time.deltaTime * _fadeMultiplier;
            yield return null;
        }
        ReleaseDecal(decal);
    }

    private DecalProjector OnCreateDecal()
    {
        GameObject newObject = 
            GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity * Quaternion.AngleAxis(90f, Vector3.right), _parent.transform);
        return newObject.GetComponent<DecalProjector>();
    }
    private void OnGetDecal(DecalProjector decalProjector)
    {
        decalProjector.gameObject.SetActive(true);
        _parent.StartCoroutine(FadeAfterTime(decalProjector));
    }
    private void OnReleaseDecal(DecalProjector decalProjector)
    {
        decalProjector.fadeFactor = 1f;
        decalProjector.gameObject.SetActive(false);
    }
    private void OnDestroyDecal(DecalProjector decalProjector)
    {
        
    }
}