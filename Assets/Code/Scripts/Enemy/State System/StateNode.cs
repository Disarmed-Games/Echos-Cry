
//Container for transitions associated to a state
//Default array represents the default transitions of a state aka transitions that need their conditions checked every frame
//Tick array represents transitions of a state that can be checked based on a tick timer rather than every frame (ex. checking player distance)
public readonly struct StateNode
{
    private readonly EnemyState _state;

    private readonly EnemyStateTransition[] _defaultArray;
    private readonly EnemyStateTransition[] _tickArray;

    public EnemyState State { get => _state; }
    public EnemyStateTransition[] DefaultArray { get => _defaultArray; }
    public EnemyStateTransition[] TickArray { get => _tickArray; }

    public StateNode(EnemyState state, EnemyStateTransition[] defaultList, EnemyStateTransition[] tickList)
    {
        _state = state;
        _defaultArray = defaultList;
        _tickArray = tickList;
    }
}