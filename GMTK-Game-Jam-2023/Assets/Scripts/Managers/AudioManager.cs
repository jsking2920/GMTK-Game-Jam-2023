using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    FMOD.Studio.EventInstance music;
    
    // Start is called before the first frame update
    void Start()
    {
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music");
        music.start();
    }

    private void OnEnable()
    {
        GameManager.ResetGame += ResetMusic;
        GameManager.StartNextSentence += OnStartNextSentence;
    }

    private void Update()
    {
        // if (!music.isValid())
        // {
        //     music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music");
        //     music.start();
        // }
    }

    private void OnStartNextSentence(int sentence)
    {
        music.setParameterByName("Intensity", sentence);
        Debug.Log(sentence);
    }

    private void OnDisable()
    {
        GameManager.ResetGame -= ResetMusic;
        GameManager.StartNextSentence -= OnStartNextSentence;
    }

    public void StartMusic()
    {
        music.setParameterByName("Started", 1);
        music.setParameterByName("Intensity", 0);
    }

    public void ResetMusic()
    {
        music.setParameterByName("Started", 0);
        music.setParameterByName("Intensity", 0);
    }
}
