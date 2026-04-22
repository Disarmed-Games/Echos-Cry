using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] GameObject dashAttackPrefab;
    DashAttack dashAttack = null;
    
    public void AddDashAttack()
    {
        dashAttack = Instantiate(dashAttackPrefab, transform).GetComponent<DashAttack>();
    }
    public DashAttack TryGetDashAttack()
    {
        return dashAttack;
    }
}
