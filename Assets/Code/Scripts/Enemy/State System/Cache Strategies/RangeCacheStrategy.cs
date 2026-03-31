using UnityEngine;
using EchosCry.Enemy.StateSystem;

[CreateAssetMenu(menuName = "Echo's Cry/Enemy/Cache Strategy/Range")]
public class RangeCacheStrategy : EnemyCacheStrategy
{
    [SerializeField] private RangeData _data;

    public override void Execute(EnemyStateCache stateCache, Enemy enemyContext)
    {
        EnemyStateTransition death_check = new(EnemyStates.Death, enemy => enemy.Health.CurrentHealth <= 0);
        EnemyStateTransition ready_to_attack = new(EnemyStates.Attack, enemy => enemy.StateData.ReadyToAttack);
        EnemyStateTransition attack_ended = new(EnemyStates.Cooldown, enemy => enemy.AttackStrategies[0].AttackEnded);
        EnemyStateTransition is_staggered = new(EnemyStates.Stagger, enemy => enemy.StateData.IsStaggered);
        EnemyStateTransition stagger_ended = new(EnemyStates.Roam, enemy => !enemy.StateData.IsStaggered);
        EnemyStateTransition cooldown_ended = new(EnemyStates.Roam, enemy => !enemy.StateData.OnCooldown);
        EnemyStateTransition player_distance_check =
            new(EnemyStates.Roam,
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
                UnityEngine.AI.NavMeshAgent agent = enemy.NavMeshAgent;
                if (agent == null) return false;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) return true;
                }
                return false;
            });

        EnemyStateHandler handler = new(enemyContext.NewStateCache);

        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Idle,
                new EnemyStateTransition[] { death_check, is_staggered },
                new EnemyStateTransition[] { player_distance_check }
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Roam,
                new EnemyStateTransition[] { death_check, is_staggered },
                new EnemyStateTransition[] { in_range_check }
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Stagger,
                new EnemyStateTransition[] { stagger_ended, death_check },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Death,
                new EnemyStateTransition[0],
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Charge,
                new EnemyStateTransition[] { death_check, ready_to_attack, is_staggered },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Attack,
                new EnemyStateTransition[] { death_check, attack_ended, is_staggered },
                new EnemyStateTransition[0]
            );
        handler.AddStateNode
            (
                enemyContext.NewStateCache,
                EnemyStates.Cooldown,
                new EnemyStateTransition[] { death_check, is_staggered, cooldown_ended },
                new EnemyStateTransition[0]
            );

        enemyContext.StateHandler = handler;
    }
}
