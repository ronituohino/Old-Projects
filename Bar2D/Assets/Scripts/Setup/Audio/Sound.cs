using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioSource source { get; set; }

    public string name;
    public AudioClip clip;

    [Space]

    public AudioMixerGroup mixerGroup;

    [Space]

    public float volume = 1f;
    public float pitch = 1f;
}