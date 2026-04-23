using AudioSystem;
using UnityEngine;

public class DroppedXP : ItemDropHandler
{
    [SerializeField] private float _xpAmount = 10f;
    private PlayerXP _xp;
    protected override void OnInteraction(Collider other)
    {
        if(_xp == null)
        {
            if(other.TryGetComponent<Player>(out Player player))
            {
                _xp = player.XP;
                _xp.IncreaseXP(_xpAmount);
            }
        }
        else _xp.IncreaseXP(_xpAmount);
    }
}
