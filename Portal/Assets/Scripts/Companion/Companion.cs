using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    protected bool mTeleport;
    public bool mAttached;
    private Rigidbody mRigidbody;
    protected AudioSource mAudioSource;
    public AudioClip mBounceSound;
    public AudioClip mNormalSound;
    public AudioClip mSlideSound;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mRigidbody = GetComponent<Rigidbody>();
    }

    public void SetTeleport(bool b)
    {
        mTeleport = true;
    }

    public void Teleport(Portal portal)
    {
        Vector3 lPosition = portal.transform.InverseTransformPoint(transform.position);
        transform.position = portal.mMirrorPortal.transform.TransformPoint(lPosition);
        Vector3 lDirection = portal.transform.InverseTransformDirection(-transform.forward);
        transform.forward = portal.mMirrorPortal.transform.TransformDirection(lDirection);

        Vector3 lVelocity = portal.transform.InverseTransformDirection(-mRigidbody.velocity);
        mRigidbody.velocity = portal.mMirrorPortal.transform.TransformDirection(lVelocity);
        transform.localScale *= portal.mMirrorPortal.transform.localScale.x / portal.transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mTeleport && other.CompareTag("Portal"))
        {
            Teleport(other.transform.GetComponent<Portal>());
            mTeleport = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.collider.tag)
        {
            case "Destroyer":
                Destroy(gameObject);
                break;
            case "Bouncer":
                mAudioSource.volume = 1;
                mAudioSource.loop = false;
                mAudioSource.clip = mBounceSound;
                mAudioSource.Play();
                break;
            case "Sliding":
                mAudioSource.volume = 1;
                mAudioSource.loop = true;
                mAudioSource.clip = mSlideSound;
                mAudioSource.Play();
                break;
            default:
                mAudioSource.volume = 1;
                mAudioSource.loop = false;
                mAudioSource.clip = mNormalSound;
                mAudioSource.Play();
                break;
                
        }
    }
}