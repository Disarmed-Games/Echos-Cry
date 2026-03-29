using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BombSpawnState : EnemyState
{
    public BombSpawnState(Enemy enemyContext) : base(enemyContext) { }

    protected override void OnEnter()
    {
        //Debug.Log("Enter Spawn");
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatIdle));
    }
}

public class BombIdleState : EnemyState
{
    private readonly BombData _configFile;

    private void CheckPlayerDistance()
    {
        if (PlayerRef.Transform == null) return;
        float playerDistance = Vector3.Distance(_enemyContext.transform.position, PlayerRef.Transform.position);
        if (playerDistance < _configFile.DistanceCheck)
        {
            _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatChase));
            return;
        }
    }

    public BombIdleState(BombData configFile, Enemy enemyContext) : base(enemyContext)
    {
        _configFile = configFile;
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

    protected override void OnTick()
    {
        CheckPlayerDistance();
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatDeath));
    }
    protected override void OnEnter()
    {
        //_enemyContext.NPCAnimator.PlayAnimation(_configFile.FlyHashCode);
    }
}

public class BombChaseState : EnemyState
{
    private void CheckNavMeshDistance()
    {
        NavMeshAgent agent = _enemyContext.NavMeshAgent;
        if (agent == null) return;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatCharge));
        }
    }
    private void SetEnemyTarget()
    {
        if (_enemyContext.NavMeshAgent == null || PlayerRef.Transform == null) return;
        _enemyContext.NavMeshAgent.SetDestination(PlayerRef.Transform.position);
    }

    public BombChaseState(Enemy enemyContext) : base(enemyContext) { }
    protected override void OnEnable()
    {
        TickManager.Instance.GetTimer(0.2f).Tick += Tick;
    }
    protected override void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.GetTimer(0.2f).Tick -= Tick;
    }

    protected override void OnEnter()
    {
        //Debug.Log("Enter Chase");
        SetEnemyTarget();
    }
    protected override void OnExit()
    {
        //Debug.Log("Exit Chase");
        _enemyContext.StopAllCoroutines();
    }
    protected override void OnTick()
    {
        CheckNavMeshDistance();
        SetEnemyTarget();
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatDeath));
    }
    public override void Update()
    {
        _enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - _enemyContext.transform.position).normalized, true);
    }
}

public class BombChargeState : EnemyState
{
    private readonly BombData _configFile;
    private IEnumerator ChargeAttackCoroutine()
    {
        yield return new WaitForSeconds(_configFile.AttackChargeTime);
        if (TempoConductor.Instance.IsOnBeat())
            _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatAttack));
        else
            _enemyContext.StartCoroutine(WaitUntilBeat());
    }
    private IEnumerator WaitUntilBeat()
    {
        yield return new WaitForEndOfFrame();
        if (TempoConductor.Instance.IsOnBeat())
            _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatAttack));
        else
            _enemyContext.StartCoroutine(WaitUntilBeat());
    }

    public BombChargeState(BombData configFile, Enemy enemyContext) : base(enemyContext)
    {
        _configFile = configFile;
    }

    protected override void OnEnter()
    {
        //Debug.Log("Enter Charge Attack State");
        _enemyContext.StartCoroutine(ChargeAttackCoroutine());
    }
    protected override void OnExit()
    {
        //Debug.Log("Exit Charge Attack State");
        _enemyContext.StopAllCoroutines();
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatDeath));
    }
    public override void Update()
    {
        _enemyContext.NPCAnimator
            .UpdateSpriteDirection((PlayerRef.Transform.position - _enemyContext.transform.position).normalized, true);
    }
}

public class BombAttackState : EnemyState
{
    private readonly BombData _configFile;
    private Vector3 attackDirection;
    private enum AttackStrats
    {
        Melee = 0,
    }
    private bool isAttacking;

    private int MeleeIndex => (int)AttackStrats.Melee;
    private IEnumerator AttackDuration()
    {
        isAttacking = true;
        yield return new WaitForSeconds(_configFile.AttackDuration);
        isAttacking = false;
        _enemyContext.StartCoroutine(AttackCooldown());
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_configFile.AttackCooldown);
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatChase));
    }

    public BombAttackState(BombData configFile, Enemy enemyContext) : base(enemyContext)
    {
        _configFile = configFile;
    }

    protected override void OnEnter()
    {
        isAttacking = true;

        _enemyContext.Rigidbody.isKinematic = false;
        attackDirection = (PlayerRef.Transform.position - _enemyContext.transform.position).normalized;
        _enemyContext.Rigidbody.AddForce(_configFile.AttackDashForce * attackDirection, ForceMode.Impulse);
        //_enemyContext.NPCAnimator.PlayAnimation(_configFile.AttackHashCode);
        _enemyContext.StartCoroutine(AttackDuration());
    }
    public override void Update()
    {
    }
    protected override void OnExit()
    {
        _enemyContext.Rigidbody.isKinematic = true;
        //_enemyContext.NPCAnimator.PlayAnimation(_configFile.FlyHashCode);
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatDeath));
    }

}

public class BombStaggerState : EnemyState
{
    private readonly BombData _configFile;
    private IEnumerator StaggerDuration()
    {
        yield return new WaitForSeconds(_configFile.StaggerDuration);
        _enemyContext.StateMachine.SwitchState(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatChase));
    }

    public BombStaggerState(BombData configFile, Enemy enemyContext) : base(enemyContext)
    {
        _configFile = configFile;
    }

    protected override void OnEnter()
    {
        //Debug.Log("Enter Stagger State");
        _enemyContext.Rigidbody.isKinematic = false;
        Vector3 direction = (PlayerRef.Transform.position - _enemyContext.transform.position).normalized;
        _enemyContext.Rigidbody.AddForce(-(_configFile.KnockbackForce * direction), ForceMode.Impulse);
        _enemyContext.StartCoroutine(StaggerDuration());
        _enemyContext.NPCAnimator.StaggerParticleStart();
    }
    protected override void OnExit()
    {
        _enemyContext.Rigidbody.isKinematic = true;
        _enemyContext.StopAllCoroutines();
        _enemyContext.NPCAnimator.StaggerParticleStop();
    }
    public override void CheckSwitch()
    {
        CheckDeath(_enemyContext.StateCache.RequestState(EnemyStateCache.EnemyStates.BatDeath));
    }
}

public class BombDeathState : EnemyState
{
    public BombDeathState(Enemy enemyContext) : base(enemyContext) { }

    protected override void OnEnter()
    {
        _enemyContext.DropsStrategy.Execute(_enemyContext.transform);
        _enemyContext.HandleDeath();
    }
}
