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

    void PlayShotLoopAudio()
    {
        source_loops = ChooseSource();
        source_loops.clip = audioClips[0];
        source_loops.priority = priority;
        source_loops.loop = true;
        source_loops.volume = volume;
        source_loops.Play();
    }

    void StopShotLoopAudio()
    {
        if (source_loops)
            source_loops.Stop();
    }

    void PlayShipMovementAudio()
    {
        if (!isPlaying)
        {
            source_starts = ChooseSource();
            source_starts.clip = audioClips[0];
            source_starts.volume = volume;
            source_starts.priority = priority;
            source_starts.loop = false;
            source_starts.Play();

            source_loops = ChooseSource();
            source_loops.clip = audioClips[1];
            source_loops.volume = volume;
            source_loops.priority = priority;
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

    void PlayDodgeSound(int sound)
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[sound];
        source_once.loop = false;
        source_once.priority = priority;
        source_once.volume = 0.2f;
        source_once.Play();
    }

    void PlayShotOnceSound()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[0];
        source_once.loop = false;
        source_once.priority = priority;
        source_once.volume = volume;
        source_once.Play();
    }

    void PlayUltraOnceSound()
    {
        source_once = ChooseSource();
        source_once.clip = audioClips[1];
        source_once.loop = false;
        source_once.priority = priority;
        source_once.volume = volume;
        source_once.Play();
    }

    void PlayChargeAudio()
    {
        source_starts = ChooseSource();
        source_starts.clip = audioClips[1];
        source_starts.loop = false;
        source_once.priority = priority;
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
        source_once.pitch -= rate / 10;
        source_once.priority = priority;
        source_once.Play();
    }

    void ResetAudioSource(AudioSource a)
    {
        a.pitch = 1;
        a.playOnAwake = false;
        a.priority = priority;
        a.loop = false;
    }


}