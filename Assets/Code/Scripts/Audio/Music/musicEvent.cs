using UnityEngine;
using UnityEngine.Audio;
/// Original Author: Victor
/// All Contributors Since Creation: Victor, Michael, Andy
/// Last Modified By: 
namespace SoundSystem
{
    [CreateAssetMenu(fileName = "MusicEvent", menuName = "Echo's Cry/Music System/Music Event")]
    public class MusicEvent : ScriptableObject
    {
        [Range(0f, 1f), SerializeField] private float musicVolume = .1f;
        [Range(0, 200f), SerializeField] private float bpm = 100f;
        [SerializeField] private AudioClip[] musicLayers;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private bool keepTrackOfBeat;

        //Public Getters
        public float Volume => musicVolume;
        public float BPM => bpm;
        public bool KeepTrackOfBeat => keepTrackOfBeat;
        public AudioClip[] Layers => musicLayers;
        public AudioMixerGroup MixerGroup => musicMixerGroup;
    }
}


