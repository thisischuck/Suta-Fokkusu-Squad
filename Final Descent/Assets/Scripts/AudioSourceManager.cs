using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public float volume;
    [Range(0, 256)]
    public int priority;

    public List<AudioClip> audioClips;

    public List<AudioSource> audioSources;

    private AudioSource source_once, source_loops, source_starts, source_ends;

    bool isPlaying;

    void Start()
    {
        audioSources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
    }

    AudioSource ChooseSource()
    {
        foreach (var src in audioSources)
        {
            if (!src.isPlaying)
            {
                ResetAudioSource(src);
                return src;
            }
        }
        return null;
    }

    #region Enemies

    void PlayEyeLaserAudio()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[0];
        source_once.loop = true;
        source_once.volume = volume;
        source_once.Play();
    }

    void StopEyeLaserAudio()
    {
        if (source_once)
            source_once.Stop();
    }

    #endregion

    #region ShipMovement

    void PlayShipMovementAudio()
    {
        if (!isPlaying)
        {
            source_starts = ChooseSource();
            source_starts.clip = audioClips[0];
            source_starts.volume = volume;
            source_starts.loop = false;
            source_starts.Play();

            source_loops = ChooseSource();
            source_loops.clip = audioClips[1];
            source_loops.volume = volume;
            source_loops.loop = true;
            source_loops.PlayScheduled(source_starts.clip.length);

            isPlaying = true;
        }
    }

    void StopShipMovementAudio()
    {
        if (source_loops)
            source_loops.Stop();
        if (source_starts)
            source_starts.Stop();

        isPlaying = false;
    }

    void PlayPlayerImpactAudio()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[2];
        source_once.loop = false;
        source_once.volume = volume;
        source_once.Play();
    }

    #endregion

    #region Weapons

    void PlayMachineGunAudio()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[0];
        source_once.loop = false;
        source_once.volume = volume;
        source_once.Play();
    }

    void PlayLaserAudio()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[0];
        source_once.loop = false;
        source_once.volume = volume;
        source_once.Play();
    }

    void PlayChargeAudio()
    {
        source_starts = ChooseSource();
        source_starts.clip = audioClips[3];
        source_starts.loop = false;
        source_starts.volume = volume;
        source_starts.Play();
    }

    void StopChargeAudio()
    {
        if (source_starts)
            source_starts.Stop();
    }

    void PlayUltraLaserAudio(float rate)
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[0];
        source_once.loop = false;
        source_once.volume = volume + rate / 5;
        Debug.Log(source_once.volume);
        source_once.pitch -= rate / 10;
        source_once.Play();
    }

    #endregion

    void ResetAudioSource(AudioSource a)
    {
        a.pitch = 1;
        a.playOnAwake = false;
        a.loop = false;
    }


}