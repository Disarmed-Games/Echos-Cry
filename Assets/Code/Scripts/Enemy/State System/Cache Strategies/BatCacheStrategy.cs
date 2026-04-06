using EchosCry.Enemy.StateSystem;
using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(menuName = "Echo's Cry/Enemy/Cache Strategy/Bat")]
public class BatCacheStrategy : EnemyCacheStrategy
{
    public override void Execute(Enemy enemyContext)
    {
        EnemyStateTransition death_check     = new(EnemyStates.Death, enemy => enemy.Health.CurrentHealth <= 0);
        EnemyStateTransition ready_to_attack = new(EnemyStates.Attack, enemy => enemy.StateData.ReadyToAttack);
        EnemyStateTransition attack_ended    = new(EnemyStates.Cooldown, enemy => enemy.AttackStrategies[0].AttackEnded);
        EnemyStateTransition is_staggered    = new(EnemyStates.Stagger, enemy => enemy.StateData.IsStaggered);
        EnemyStateTransition stagger_ended   = new(EnemyStates.Pursue, enemy => !enemy.StateData.IsStaggered);
        EnemyStateTransition cooldown_ended  = new(EnemyStates.Pursue, enemy => !enemy.StateData.OnCooldown);
        EnemyStateTransition player_distance_check = 
            new (EnemyStates.Pursue, 
            enemy =>
            {
                if (PlayerRef.Transform == null) return false;
                float playerDistance = Vector3.Distance(enemy.transform.position, PlayerRef.Transform.position);
                if (playerDistance < enemy.Data.DistanceCheck) return true;
                return false;
            });
        EnemyStateTransition in_range_check =
            new(EnemyStates.Charge,
            enemy =>
            {
                NavMeshAgent agent = enemy.NavMeshAgent;
                if (agent == null) return false;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) return true;
                }
                return false;
            });

        EnemyStateHandler handler = new(enemyContext.StateCache);

        handler.AddStateNode
            (
                enemyContext.StateCache, 
                EnemyStates.Idle, 
                new EnemyStateTransition[] {death_check, is_staggered}, 
                new EnemyStateTransition[] { player_distance_check }
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Pursue,
                new EnemyStateTransition[] {death_check, is_staggered},
                new EnemyStateTransition[] { in_range_check }
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Stagger,
                new EnemyStateTransition[] { stagger_ended, death_check },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Death,
                new EnemyStateTransition[0],
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Charge,
                new EnemyStateTransition[] {death_check, ready_to_attack , is_staggered },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Attack,
                new EnemyStateTransition[] {death_check, attack_ended, is_staggered },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.StateCache,
                EnemyStates.Cooldown,
                new EnemyStateTransition[] { death_check, is_staggered, cooldown_ended },
                new EnemyStateTransition[0]
            );

        enemyContext.StateHandler = handler;
    }
}
