using UnityEngine;

public abstract class MovingPlatform : MonoBehaviour
{
    protected bool mMoving;
    private AudioSource mAudioSource;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mMoving = true;
            other.transform.parent = transform;
            mAudioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mMoving = true;
            other.transform.parent = null;
        }
    }
}