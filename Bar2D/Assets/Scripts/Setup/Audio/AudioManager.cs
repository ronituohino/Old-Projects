using UnityEngine;
using System;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.playOnAwake = false;

            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    //Unoptimized for sounds that get played a lot, make seperate functions for those sounds
    public void PlaySound(string soundName)
    {
        Sound sound = Array.Find(sounds, s => s.name == soundName);

        if(sound != null)
        {
            sound.source.Play();
        } else
        {
            print($"Sound ({soundName}) not found!");
        }
    }
}