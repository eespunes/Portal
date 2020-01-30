using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public UnityEvent mEvent;
    public bool onlyByLaser;
    private AudioSource mAudioSource;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!onlyByLaser)
            switch (other.tag)
            {
                case "Companion":
                    PlayEvent();
                    break;
            }
    }

    public void PlayEvent()
    {
        mAudioSource.Play();
        mEvent.Invoke();
    }
}