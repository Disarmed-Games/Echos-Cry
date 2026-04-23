using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageLabelManager : NonSpawnableSingleton<DamageLabelManager>
{
    private ObjectPool<DamageLabel> damageLabelPopupPool;
    private List<DamageLabel> allDamageLabels;

    //Find another way to pass damage label to manager
    [SerializeField] private DamageLabel damageLabelPrefab;
    [Range(0.2f, 1.5f), SerializeField] private float _displayLength = 1f;

    protected override void OnAwake()
    {
        allDamageLabels = new List<DamageLabel>();
        
        damageLabelPopupPool = new ObjectPool<DamageLabel>(
            createFunc: CreateLabel,
            actionOnGet: GetLabel,
            actionOnRelease: ReleaseLabel,
            actionOnDestroy: DestroyLabel
        );
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        DestroyAllLabels();
        allDamageLabels.Clear();
        damageLabelPopupPool.Clear();
    }

    public void SpawnPopup(float damage, Vector3 position, Color color)
    {
        Vector3 screenPosition = CameraManager.MainCamera.WorldToScreenPoint(position);
        screenPosition.z = 0;
        bool direction = screenPosition.x < Screen.width * 0.5f;

        GetAndDisplayPopup(damage, screenPosition, direction, color);
    }

    private void GetAndDisplayPopup(float damage, Vector3 position, bool direction, Color color)
    {
        damageLabelPopupPool.Get().Display(damage, position, direction, color);
    }
    private void DestroyAllLabels()
    {
        for(int i = 0;i < allDamageLabels.Count; i++)
        {
            Destroy(allDamageLabels[i].gameObject);
        }
    }
    private IEnumerator ReturnDamageLabelToPool(DamageLabel damageLabel)
    {
        yield return new WaitForSeconds(_displayLength);
        damageLabelPopupPool.Release(damageLabel);
    }
    private DamageLabel CreateLabel()
    {
        DamageLabel damageLabel = Instantiate(damageLabelPrefab, transform);
        allDamageLabels.Add(damageLabel);
        damageLabel.Initialize(_displayLength, damageLabelPopupPool);
        return damageLabel;
    }
    private void GetLabel(DamageLabel damageLabel)
    {
        damageLabel.gameObject.SetActive(true);
        allDamageLabels.Add(damageLabel);
        StartCoroutine(ReturnDamageLabelToPool(damageLabel));
    }
    private void ReleaseLabel(DamageLabel damageLabel)
    {
        damageLabel.gameObject.SetActive(false);
        allDamageLabels.Remove(damageLabel);
    }
    private void DestroyLabel(DamageLabel damageLabel)
    {
        Destroy(damageLabel.gameObject);
    }
}