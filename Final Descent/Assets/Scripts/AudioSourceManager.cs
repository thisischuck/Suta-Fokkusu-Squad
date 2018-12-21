using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public List<AudioClip> audioClips;

    private AudioSource source;

    private ParticleSystem ps;

    public bool audioIsPlaying;

    void Start()
    {
        audioIsPlaying = false;
        source = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps.isPlaying && !audioIsPlaying)
        {
            source.clip = audioClips[0];
            source.Play();
            source.loop = true;
            audioIsPlaying = true;
        }

        if (ps.isStopped)
        {
            source.Stop();
            audioIsPlaying = false;
        }
    }

}