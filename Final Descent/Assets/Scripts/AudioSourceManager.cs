using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public enum States
    {
        Eye,
        Player,
        Weapons
    }

    public float volume;
    public float priority;

    public List<AudioClip> audioClips;
    public List<AudioSource> audioSources;

    private AudioSource source;
    public List<ParticleSystem> Systems;

    bool audioIsLooping;
    bool audioIsStarting;
    bool audioIsEnding;

    AudioSource sourceIsLooping;
    AudioSource sourceIsStarting;
    AudioSource sourceIsEnding;

    public States states;

    void Start()
    {
        StatesStartManager();
    }

    void Update()
    {
        StatesUpdateManager();
    }


    #region Eye

    void StartEye()
    {
        audioIsLooping = false;
        source = GetComponent<AudioSource>();
    }

    void UpdateEye()
    {
        if (Systems[0].isPlaying && !audioIsLooping)
        {
            source.clip = audioClips[0];
            source.Play();
            source.loop = true;
            audioIsLooping = true;
            source.volume = volume;
        }

        if (Systems[0].isStopped)
        {
            source.Stop();
            audioIsLooping = false;
        }
    }

    #endregion
    #region Player

    void StartPlayer()
    {
        audioIsLooping = false;
        audioSources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
    }

    void UpdatePlayer()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            audioIsStarting = true;
        }
        else
        {
            audioIsStarting = false;
            audioIsLooping = false;

            if (sourceIsLooping)
            {
                sourceIsLooping.Stop();
                sourceIsLooping = null;
            }

            if (sourceIsStarting)
            {
                sourceIsStarting.Stop();
                sourceIsStarting = null;
            }
        }


        if (audioIsStarting && !sourceIsStarting)
        {
            sourceIsStarting = ChooseSource();
            audioIsStarting = false;
            sourceIsStarting.clip = audioClips[0];
            sourceIsStarting.volume = volume;
            sourceIsStarting.loop = false;
            audioIsLooping = true;
            sourceIsStarting.Play();

            sourceIsLooping = ChooseSource();
            sourceIsLooping.clip = audioClips[1];
            sourceIsLooping.volume = volume;
            sourceIsLooping.loop = true;
            sourceIsLooping.PlayScheduled(sourceIsStarting.clip.length);
        }
    }

    #endregion
    #region Weapons
    // One Source For Each Weapon
    void StartWeapons()
    {
        source = GetComponent<AudioSource>();
    }

    void PlayMachineGunAudio()
    {
        source.clip = audioClips[0];
        source.loop = false;
        source.volume = volume;
        source.Play();
    }

    void UpdateWeapons()
    {

    }

    #endregion
    private void StatesStartManager()
    {
        switch (states)
        {
            case States.Eye:
                StartEye();
                break;
            case States.Player:
                StartPlayer();
                break;
            case States.Weapons:
                StartWeapons();
                break;
        }
    }

    private void StatesUpdateManager()
    {
        switch (states)
        {
            case States.Eye:
                UpdateEye();
                break;
            case States.Player:
                UpdatePlayer();
                break;
            case States.Weapons:
                UpdateWeapons();
                break;
        }
    }

    AudioSource ChooseSource()
    {
        foreach (var src in audioSources)
        {
            if (!src.isPlaying)
                return src;
        }
        return null;
    }

}