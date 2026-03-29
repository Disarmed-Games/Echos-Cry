using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnemyStateCache;

public class RangeSpawnState : EnemyState
{
    public RangeSpawnState(Enemy enemyContext) : base(enemyContext) { }

    protected override void OnEnter()
    {
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeIdle));
    }
}
public class RangeIdleState : EnemyState
{
    private RangeData _config;

    public RangeIdleState(RangeData config, Enemy enemyContext) : base(enemyContext)
    {
        _config = config;
    }
    protected override void OnEnable()
    {
        TickManager.Instance.GetTimer(0.2f).Tick += Tick;
    }
    protected override void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.GetTimer(0.2f).Tick -= Tick;
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStates.RangeDeath));
    }

    protected override void OnTick()
    {
        float distance = Vector3.Distance(PlayerRef.Transform.position, _enemyContext.transform.position);
        if (distance <= _config.DistanceCheck) 
            _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeRoam));
    }
}
public class RangeRoamState : EnemyState
{
    RangeData _config;
    public RangeRoamState(RangeData config, Enemy enemyContext) : base(enemyContext) 
    {
        _config = config;
    }
    protected override void OnEnable()
    {
        TickManager.Instance.GetTimer(0.2f).Tick += Tick;
    }
    protected override void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.GetTimer(0.2f).Tick -= Tick;
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStates.RangeDeath));
    }
    public override void Update()
    {
        _enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - _enemyContext.transform.position).normalized, true);
    }
    protected override void OnEnter()
    {
        //Debug.Log("Enter Roam");
        Vector3 point = Random.onUnitSphere * _config.DistanceFromPlayer;
        point.y = 0;
        Vector3 destination = PlayerRef.Transform.position + point;
        _enemyContext.NavMeshAgent?.SetDestination(destination);
    }
    protected override void OnTick()
    {
        NavMeshAgent agent = _enemyContext.NavMeshAgent;
        if (agent == null || !agent.isOnNavMesh) return;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) 
                _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeCharge));
        }
    }
}
public class RangeChargeAttackState : EnemyState
{
    RangeData _config;
    public RangeChargeAttackState(RangeData config, Enemy enemyContext) : base(enemyContext) 
    { 
        _config = config;
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStates.RangeDeath));
    }
    protected override void OnEnter()
    {
        //Debug.Log("Enter Charge");
        _enemyContext.StartCoroutine(ChargeDurationCoroutine());
    }
    public override void Update()
    {
        _enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - _enemyContext.transform.position).normalized, true);
    }
    protected override void OnExit()
    {
        _enemyContext.StopAllCoroutines();
    }
    private IEnumerator ChargeDurationCoroutine()
    {
        yield return new WaitForSeconds(_config.AttackChargeTime);
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeAttack));
    }
}
public class RangeAttackState : EnemyState
{
    RangeData _config;
    int count;

    public RangeAttackState(RangeData config, Enemy enemyContext) : base(enemyContext) 
    { 
        _config = config;
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStates.RangeDeath));
    }
    protected override void OnEnter()
    {
        //Debug.Log("Enter Attack");
        count = 0;
        _enemyContext.AttackStrategies[0].Execute(10f, GetDirection(), _enemyContext.transform);
        _enemyContext.SoundStrategy.Execute(_enemyContext.SoundConfig.AttackSFX, _enemyContext.transform, 0);
    }
    public override void Update()
    {
        _enemyContext.NPCAnimator.UpdateSpriteDirection(GetDirection(), true);
        if (_enemyContext.AttackStrategies[0].AttackEnded) _enemyContext.StartCoroutine(AttackCooldownCoroutine());
    }
    private Vector3 GetDirection()
    {
        return (PlayerRef.Transform.position - _enemyContext.transform.position).normalized;
    }
    protected override void OnExit()
    {
        _enemyContext.AttackStrategies[0].StopAllCoroutines();
        _enemyContext.StopAllCoroutines();
    }
    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(_config.AttackCooldown);
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeRoam));
    }
    private IEnumerator BetweenAttackPauseCoroutine()
    {
        yield return new WaitForSeconds(TempoConductor.Instance.TimeBetweenBeats);
       
        _enemyContext.StartCoroutine(AttackCooldownCoroutine());
        //else
        //{
        //    _enemyContext.AttackStrategies[0].Execute(_config.BaseDamage, GetDirection(), _enemyContext.transform);
        //    _enemyContext.SoundStrategy.Execute(_enemyContext.SoundConfig.AttackSFX, _enemyContext.transform, 0);
        //    count++;
        //}
    }
}
public class RangeStaggerState : EnemyState
{
    RangeData _config;
    public RangeStaggerState(RangeData config, Enemy enemyContext) : base(enemyContext) 
    { 
        _config = config;
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStates.RangeDeath));
    }
    protected override void OnEnter()
    {
        //Debug.Log("Enter Stagger");
        if (_enemyContext.NavMeshAgent.hasPath) _enemyContext.NavMeshAgent.ResetPath();
        _enemyContext.Rigidbody.isKinematic = false;
        Vector3 direction = (PlayerRef.Transform.position - _enemyContext.transform.position).normalized;
        _enemyContext.Rigidbody.AddForce(-(_config.KnockbackForce * direction), ForceMode.Impulse);
        _enemyContext.StartCoroutine(StaggerDurationCoroutine());
        _enemyContext.NPCAnimator.StaggerParticleStart();
    }
    protected override void OnExit()
    {
        _enemyContext.Rigidbody.isKinematic = true;
        _enemyContext.StopAllCoroutines();
        _enemyContext.NPCAnimator.StaggerParticleStop();
    }
    private IEnumerator StaggerDurationCoroutine()
    {
        yield return new WaitForSeconds(_config.StaggerDuration);
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStates.RangeRoam));
    }
}
public class RangeDeathState : EnemyState
{
    public RangeDeathState(Enemy enemyContext) : base(enemyContext) { }

    protected override void OnEnter()
    {
        _enemyContext.DropsStrategy.Execute(_enemyContext.transform);
        _enemyContext.HandleDeath();
    }
}