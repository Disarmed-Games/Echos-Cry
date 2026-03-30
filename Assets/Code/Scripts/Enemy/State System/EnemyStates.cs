namespace EchosCry.Enemy.StateSystem
{
    public enum EnemyStates
    {
        Unassigned = 0,
        Spawn, Death, Idle, Stagger, Pursue, Attack, Charge, Cooldown
        ////Bat Enemy
        //BatSpawn, BatStagger, BatDeath, BatCharge, BatAttack, BatIdle, BatChase,
        ////Range Enemy
        //RangeSpawn, RangeStagger, RangeDeath, RangeCharge, RangeAttack, RangeIdle, RangeRoam,
        ////Walker Enemy
        //WalkerSpawn, WalkerStagger, WalkerDeath, WalkerJump, WalkerAttack, WalkerIdle, WalkerChase,
        ////Slime Enemy
        //SlimeSpawn, SlimeStagger, SlimeDeath, SlimeCharge, SlimeAttack, SlimeIdle, SlimeChase,
        ////Bomb Enemy
    }
}