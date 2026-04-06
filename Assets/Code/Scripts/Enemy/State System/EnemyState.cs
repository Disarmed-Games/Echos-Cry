
//Systems acting on enemies
//States are simply the behaviors acting on enemy data
using System.Collections;
using UnityEngine;
using EchosCry.Enemy.Animation;

public abstract class EnemyState
{
    public virtual void Update(Enemy enemyContext) { }
    public virtual void FixedUpdate(Enemy enemyContext) { }
    public virtual void Enter(Enemy enemyContext) { }
    public virtual void Exit(Enemy enemyContext) { }
    public virtual void Tick(Enemy enemyContext) { }
}

public class SpawnEnemyState : EnemyState
{

}

public class IdleEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Idle");
        enemyContext.NPCAnimator.PlayAnimation(HashCodes.IdleHashCode);
    }
}

public class PursueEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Pursue");
        
        SetEnemyTarget(enemyContext);
        enemyContext.StartCoroutine(UpdateTarget(enemyContext));
        enemyContext.NPCAnimator.PlayAnimation(HashCodes.MoveHashCode);
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.StopAllCoroutines();
    }
    public override void Update(Enemy enemyContext)
    {
        enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - enemyContext.transform.position).normalized);
    }
    private void SetEnemyTarget(Enemy enemyContext)
    {
        if (enemyContext.NavMeshAgent == null || PlayerRef.Transform == null) return;
        enemyContext.NavMeshAgent.SetDestination(PlayerRef.Transform.position);
    }
    private IEnumerator UpdateTarget(Enemy enemyContext)
    {
        yield return new WaitForSeconds(0.2f);
        SetEnemyTarget(enemyContext);
        enemyContext.StartCoroutine(UpdateTarget(enemyContext));
    }
}

public class StaggerEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Stagger");
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
        EnemyStateData data = enemyContext.StateData;
        data.IsStaggered = false;
        enemyContext.StateData = data;
    }
}

public class ChargeEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Charge");
        
        enemyContext.StateData.ReadyToAttack = false;
        enemyContext.StartCoroutine(ChargeAttackCoroutine(enemyContext));
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.StopAllCoroutines();
    }
    public override void Update(Enemy enemyContext)
    {
        enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - enemyContext.transform.position).normalized);
    }
    private IEnumerator ChargeAttackCoroutine(Enemy enemyContext)
    {
        yield return new WaitForSeconds(enemyContext.Data.AttackChargeTime);
        if (TempoConductor.Instance.IsOnBeat())
        {
            //Debug.Log("Ready for attack");
            enemyContext.StateData.ReadyToAttack = true;
        }
        else enemyContext.StartCoroutine(WaitUntilBeat(enemyContext));
    }
    private IEnumerator WaitUntilBeat(Enemy enemyContext)
    {
        while (!TempoConductor.Instance.IsOnBeat())
        {
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Ready for attack");
        enemyContext.StateData.ReadyToAttack = true;
    }
}

public class AttackEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Attack");

        Vector3 attackDirection = (PlayerRef.Transform.position - enemyContext.transform.position).normalized;
        enemyContext.NPCAnimator.PlayAnimation(HashCodes.AttackHashCode);
        enemyContext.SoundStrategy.Execute(enemyContext.SoundConfig.AttackSFX, enemyContext.transform, 0f);
        enemyContext.AttackStrategies[0].Execute(enemyContext.Data.BaseDamage, attackDirection, enemyContext.transform);
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.Rigidbody.isKinematic = true;
        enemyContext.AttackStrategies[0].StopAllCoroutines();
    }
}

public class CooldownEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Cooldown");

        enemyContext.StateData.OnCooldown = true;
        enemyContext.StartCoroutine(Cooldown(enemyContext));
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.StopAllCoroutines();
    }
    private IEnumerator Cooldown(Enemy enemyContext)
    {
        yield return new WaitForSeconds(enemyContext.Data.AttackCooldown);
        enemyContext.StateData.OnCooldown = false;
    }
}

public class DeathEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Death");
        enemyContext.DropsStrategy.Execute(enemyContext.transform);
        enemyContext.HandleDeath();
    }
}
public class RoamEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        //Debug.Log("Roaming");
        SetEnemyTarget(enemyContext);
        enemyContext.NPCAnimator.PlayAnimation(HashCodes.MoveHashCode);
    }
    public override void Update(Enemy enemyContext)
    {
        enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - enemyContext.transform.position).normalized);
    }
    private void SetEnemyTarget(Enemy enemyContext)
    {
        if (enemyContext.NavMeshAgent == null || PlayerRef.Transform == null) return;
        enemyContext.NavMeshAgent.SetDestination(enemyContext.TargetStrategy[0].Execute(PlayerRef.Transform));
    }
}

public class FuseEnemyState : EnemyState
{
    public override void Enter(Enemy enemyContext)
    {
        enemyContext.StateData.ReadyToAttack = false;
        enemyContext.StartCoroutine(ChargeAttackCoroutine(enemyContext));
        enemyContext.NavMeshAgent.speed /= 1.25f;
        SetEnemyTarget(enemyContext);
        enemyContext.StartCoroutine(UpdateTarget(enemyContext));
    }
    public override void Exit(Enemy enemyContext)
    {
        enemyContext.StopAllCoroutines();
    }
    public override void Update(Enemy enemyContext)
    {
        enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - enemyContext.transform.position).normalized);

    }
    private IEnumerator ChargeAttackCoroutine(Enemy enemyContext)
    {
        yield return new WaitForSeconds(enemyContext.Data.AttackChargeTime);
        if (TempoConductor.Instance.IsOnBeat())
        {
            //Debug.Log("Ready for attack");
            enemyContext.StateData.ReadyToAttack = true;
        }
        else enemyContext.StartCoroutine(WaitUntilBeat(enemyContext));
    }
    private IEnumerator WaitUntilBeat(Enemy enemyContext)
    {
        while (!TempoConductor.Instance.IsOnBeat())
        {
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Ready for attack");
        enemyContext.StateData.ReadyToAttack = true;
    }
    private void SetEnemyTarget(Enemy enemyContext)
    {
        if (enemyContext.NavMeshAgent == null || PlayerRef.Transform == null) return;
        enemyContext.NavMeshAgent.SetDestination(PlayerRef.Transform.position);
    }
    private IEnumerator UpdateTarget(Enemy enemyContext)
    {
        yield return new WaitForSeconds(0.2f);
        SetEnemyTarget(enemyContext);
        enemyContext.StartCoroutine(UpdateTarget(enemyContext));
    }
}