using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject mDoor;
    private AudioSource mAudioSource;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }
    public void OpenDoor()
    {
        mAudioSource.Play();
        mDoor.SetActive(false);
    }
}