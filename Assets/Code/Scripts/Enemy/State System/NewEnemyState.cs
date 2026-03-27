public abstract class NewEnemyState
{
    public virtual void Update(Enemy enemyContext) { }
    public virtual void FixedUpdate(Enemy enemyContext) { }
    public virtual void Enter(Enemy enemyContext) { }
    public virtual void Exit(Enemy enemyContext) { }
}

public class IdleEnemyState : NewEnemyState
{

}

public class SpawnEnemyState : NewEnemyState
{

}

public class StaggerEnemyState : NewEnemyState
{

}

public class DeathEnemyState : NewEnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        enemyContext.DropsStrategy.Execute(enemyContext.transform);
        enemyContext.HandleDeath();
    }
}