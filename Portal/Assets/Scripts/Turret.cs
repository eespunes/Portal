using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Turret : Companion
{
    public LineRenderer mLineRenderer;
    public LayerMask mCollisionLayerMask;
    public float mMaxDistance = 250.0f;
    public float mAngleLaserActive = 60.0f;

    public AudioClip mLaserSound;

    void Update()
    {
        Vector3 lEndRaycastPosition = Vector3.forward * mMaxDistance;
        RaycastHit lRaycastHit;
        if (Physics.Raycast(new Ray(mLineRenderer.transform.position,
                mLineRenderer.transform.forward), out lRaycastHit, mMaxDistance,
            mCollisionLayerMask.value))
        {
            lEndRaycastPosition = lRaycastHit.point;
            switch (lRaycastHit.collider.tag)
            {
                case "Player":
                    SceneManager.LoadScene("GameOver");
                    break;
                case "Refraction Companion":
                    var lCompanion = lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>();
                    if (!lCompanion.mAttached)
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreateRefraction(lRaycastHit.point);
                    else
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreatePreviewRefraction(lRaycastHit.point);
                    break;
                case "Turret":
                    GameObject.Find("Player").GetComponent<Weapon>().DetachObject(0);
                    Destroy(lRaycastHit.collider.gameObject);
                    break;
                case "Portal":
                    lRaycastHit.collider.gameObject.GetComponent<Portal>().CreateRefraction(lRaycastHit.point,false);
                    break;
            }
        }

        mLineRenderer.SetPosition(0, mLineRenderer.transform.position);
        mLineRenderer.SetPosition(1, lEndRaycastPosition);


        float lDotAngleLaserActive = Mathf.Cos(mAngleLaserActive * Mathf.Deg2Rad * 0.5f);
        bool lRayActive = Vector3.Dot(transform.up, Vector3.up) > lDotAngleLaserActive;
        mLineRenderer.gameObject.SetActive(lRayActive);

        if (!mLineRenderer.gameObject.activeInHierarchy)
            mAudioSource.Stop();
        else if (!mAudioSource.isPlaying)
        {
            mAudioSource.volume = .5f;
            mAudioSource.loop = true;
            mAudioSource.clip = mLaserSound;
            mAudioSource.Play();
        }
    }
}