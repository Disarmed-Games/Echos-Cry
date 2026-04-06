
using System.Collections.Generic;
using EchosCry.Enemy.StateSystem;

public class EnemyStateCache
{
    private readonly Dictionary<EnemyStates, EnemyState> _stateCache;

    public EnemyStateCache()
    {
        _stateCache = new()
        {
            //Init all states
            { EnemyStates.Death   , new DeathEnemyState()    },
            { EnemyStates.Spawn   , new SpawnEnemyState()    },
            { EnemyStates.Idle    , new IdleEnemyState()     },
            { EnemyStates.Stagger , new StaggerEnemyState()  },
            { EnemyStates.Pursue  , new PursueEnemyState()   },
            { EnemyStates.Attack  , new AttackEnemyState()   },
            { EnemyStates.Charge  , new ChargeEnemyState()   },
            { EnemyStates.Cooldown, new CooldownEnemyState() },
            { EnemyStates.Roam    , new RoamEnemyState()     },
            { EnemyStates.Fuse    , new FuseEnemyState() },
        };
    }
    
    public EnemyState RequestState(EnemyStates requestedState)
    {
        if (!_stateCache.ContainsKey(requestedState)) return null;
        else return _stateCache[requestedState];
    }
}