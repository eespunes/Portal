using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public
    class CompanionSpawner : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionPrefab;
    public KeyCode mKey;
    private AudioSource mAudioSource;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public void Spawn()
    {
        Instantiate(m_CompanionPrefab, m_SpawnPosition.position,
            m_SpawnPosition.rotation, null);
        mAudioSource.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")&&Input.GetKeyDown(mKey))
        {
            Spawn();
        }
    }
}