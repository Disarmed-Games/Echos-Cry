using System.Collections;
using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    [SerializeField] private float bpm;

    void Start()
    {
        BeatManager.Instance.PlaySong(song, bpm);
    }
}
