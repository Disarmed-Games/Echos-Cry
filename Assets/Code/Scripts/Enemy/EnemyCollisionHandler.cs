using UnityEngine;
using static Enemy;

//This will be placed on every enemy to handle collision with an attack from player

//VERY VERY VERY IMPORTANT NOTE: this will only register collisions from colliders with the layer PlayerAttack placed on them
// so place PlayerAttack on the player's attack handler or whatever handles weapon collision

//This is used to increase combo meter/multiplier and set enemy to stagger state

//[RequireComponent(typeof(Rigidbody))]
//public class EnemyCollisionHandler : MonoBehaviour
//{
//    protected void OnTriggerEnter(Collider collision)
//    {
//        if(collision.TryGetComponent<WeaponCollider>(out WeaponCollider handler))
//        {
//            PlayerComboMeter.AddToComboMeter(handler.AttackDamage);
//            PlayerComboMeter.UpdateComboMultiplier();
//            _enemyCollider.enabled = false;
//            //_enemyManager.SwitchState(enemyStaggerState);
//        }
//    }
//    private void ResetColliderBool()
//    {
//        _enemyCollider.enabled = true;
//    }

//    private void Awake()
//    {
//        _enemyManager = GetComponent<Enemy>();
//        _enemyCollider = GetComponent<Collider>();
//    }
    
//    private Collider _enemyCollider;
//    protected Enemy _enemyManager;
//}
