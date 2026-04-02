public class EnemyStateData
{
    public bool IsStaggered;
    public bool ReadyToAttack;
    public bool OnCooldown;
    public EnemyStateData()
    {
        IsStaggered = false;
        ReadyToAttack = false;
        OnCooldown = false;
    }
    public void Reset()
    {
        IsStaggered=false;
        ReadyToAttack=false;
        OnCooldown=false;
    }
}
