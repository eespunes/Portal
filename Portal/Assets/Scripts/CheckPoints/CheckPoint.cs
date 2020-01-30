using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CheckPoint : MonoBehaviour
{

    private AudioSource mAudioSource;
    private Light mLight1;
    private Light mLight2;
    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mLight1 = transform.GetChild(0).GetComponent<Light>();
        mLight2 = transform.GetChild(1).GetComponent<Light>();
        if(SingletonCheckPoint.getInstance().getPosition().Equals(transform.position))
        {
            mLight1.color = Color.green;
            mLight2.color=Color.green;
        }
    }

    private void ActivateCheckPoint()
    {
        mLight1.color=Color.green;
        mLight2.color=Color.green;
        SingletonCheckPoint.getInstance().setPosition(transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCheckPoint();
            mAudioSource.Play();
        }
    }
}
