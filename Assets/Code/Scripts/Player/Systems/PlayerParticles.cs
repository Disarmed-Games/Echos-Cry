using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    //Particles
    [SerializeField] private ParticleSystem _experienceParticles;
    [SerializeField] private ParticleSystem _dashParticles;

    //Channels
    [SerializeField] private IntEventChannel _levelUpChannel;

    private void OnEnable()
    {
        _levelUpChannel.Channel += StartExperienceParticles;
    }
    private void OnDisable()
    {
        _levelUpChannel.Channel -= StartExperienceParticles;
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
