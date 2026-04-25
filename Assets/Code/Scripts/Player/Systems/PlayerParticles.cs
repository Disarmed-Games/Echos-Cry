using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    //Particles
    [SerializeField] private ParticleSystem _experienceParticles;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private ParticleSystem _noteHitParicles;

    //Channels
    [SerializeField] private IntEventChannel _levelUpChannel;
    [SerializeField] private InputTranslator _inputTranslator;

    private void OnEnable()
    {
        _levelUpChannel.Channel += StartExperienceParticles;
        _inputTranslator.OnPrimaryActionEvent += StartHitNoteParticles;
        _inputTranslator.OnSecondaryActionEvent += StartHitNoteParticles;
        _inputTranslator.OnSpecialAttackEvent += StartHitNoteParticles;
    }
    private void OnDisable()
    {
        _levelUpChannel.Channel -= StartExperienceParticles;
        _inputTranslator.OnPrimaryActionEvent -= StartHitNoteParticles;
        _inputTranslator.OnSecondaryActionEvent -= StartHitNoteParticles;
        _inputTranslator.OnSpecialAttackEvent -= StartHitNoteParticles;
    }

    private void StartHitNoteParticles(bool isPressed)
    {
        if (!isPressed) return;
        _noteHitParicles.Play();
    }
    private void StartExperienceParticles(int level)
    {
        _experienceParticles.Play();
    }
    
    public void StartDashParticles()
    {
        _dashParticles.Play();
    }
}
