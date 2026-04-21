using AudioSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Echo's Cry/SFX/SFX Config")]
public class SFXConfig : ScriptableObject
{
    [SerializeField] private soundEffect _footstepSFX;
    [SerializeField] private soundEffect _dashSFX;
    [SerializeField] private soundEffect _hurtEffect;
    [SerializeField] private soundEffect _healEffect;

    [SerializeField] soundEffect _excellentSFX;
    [SerializeField] soundEffect _goodSFX;
    [SerializeField] soundEffect _missSFX;
    public soundEffect FootstepSFX { get { return _footstepSFX; } }
    public soundEffect DashSFX { get => _dashSFX; }
    public soundEffect HurtEffect { get => _hurtEffect; }
    public soundEffect HealEffect { get => _healEffect; }

    public soundEffect ExcellentSFX { get => _excellentSFX; }
    public soundEffect GoodSFX { get => _goodSFX; }
    public soundEffect MissSFX { get => _missSFX; }
}
