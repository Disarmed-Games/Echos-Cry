
//Systems acting on enemies
//States are simply the behaviors acting on enemy data
using Codice.CM.Client.Differences.Graphic;
using System.Collections;
using UnityEngine;

public abstract class NewEnemyState
{
    public virtual void Update(Enemy enemyContext) { }
    public virtual void FixedUpdate(Enemy enemyContext) { }
    public virtual void Enter(Enemy enemyContext) { }
    public virtual void Exit(Enemy enemyContext) { }
}

public class IdleEnemyState : NewEnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        enemyContext.NPCAnimator.PlayAnimation(EchosCry.Enemy.Animation.HashCodes.IdleHashCode);
    }
}

public class SpawnEnemyState : NewEnemyState
{

}

public class StaggerEnemyState : NewEnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Enter Stagger State");
        enemyContext.Rigidbody.isKinematic = false;
        Vector3 direction = (PlayerRef.Transform.position - enemyContext.transform.position).normalized;
        enemyContext.Rigidbody.AddForce(-(enemyContext.Data.KnockbackForce * direction), ForceMode.Impulse);
        enemyContext.NPCAnimator.StaggerParticleStart();
        enemyContext.StartCoroutine(StaggerDuration(enemyContext));
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.Rigidbody.isKinematic = true;
        enemyContext.NPCAnimator.StaggerParticleStop();
        enemyContext.StopAllCoroutines();
    }
    private IEnumerator StaggerDuration(Enemy enemyContext)
    {
        //Need to pass time
        yield return new WaitForSeconds(enemyContext.Data.StaggerDuration);
        enemyContext.StateData.IsStaggered = false;
    }
}

public class DeathEnemyState : NewEnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        enemyContext.DropsStrategy.Execute(enemyContext.transform);
        enemyContext.HandleDeath();
    }
}