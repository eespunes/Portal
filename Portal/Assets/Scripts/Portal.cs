using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Portal : MonoBehaviour
{
    private Transform mPlayerCamera;
    public Portal mMirrorPortal;
    public Camera mPortalCamera;
    public float mNearClipOffset = 0.5f;
    public LayerMask mLayerMask;

    private AudioSource mAudioSource;
    public LineRenderer mLineRenderer;
    public float mMaxDistance = 200.0f;
    private bool mCreateRefraction;
    public AudioClip mLaserSound;

    private void Start()
    {
        mPlayerCamera = GameObject.Find("Player").transform.GetChild(1).GetChild(0);
        ResetCamera();
        mAudioSource = GetComponent<AudioSource>();
    }

    public void ResetCamera()
    {
        mPortalCamera.transform.localPosition = Vector3.zero;
        mPortalCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        mLineRenderer.enabled = mCreateRefraction;
        mCreateRefraction = false;

        CameraMovement();
    }

    private void OnEnable()
    {
        if (mMirrorPortal.isActiveAndEnabled)
        {
            gameObject.GetComponent<Collider>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            mMirrorPortal.gameObject.GetComponent<Collider>().enabled = true;
            mMirrorPortal.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void CameraMovement()
    {
        Vector3 lEulerAngles = transform.rotation.eulerAngles;
        Quaternion lRotation = Quaternion.Euler(lEulerAngles.x, lEulerAngles.y + 180.0f, lEulerAngles.z);
        Matrix4x4 lWorldMatrix = Matrix4x4.TRS(transform.position, lRotation, transform.localScale);
        Vector3 lReflectedPosition = lWorldMatrix.inverse.MultiplyPoint3x4(mPlayerCamera.position);
        Vector3 lReflectedDirection = lWorldMatrix.inverse.MultiplyVector(mPlayerCamera.forward);
        mMirrorPortal.mPortalCamera.transform.position = mMirrorPortal.transform.TransformPoint(lReflectedPosition);
        mMirrorPortal.mPortalCamera.transform.forward =
            mMirrorPortal.transform.TransformDirection(lReflectedDirection);
        mPortalCamera.nearClipPlane = Vector3.Distance(mPortalCamera.transform.position, transform.position) +
                                      mNearClipOffset;
    }

    public void CreateRefraction(Vector3 position, bool activatedByPortal)
    {
        if (activatedByPortal)
        {
            mLineRenderer.material.color = Color.red;
            mCreateRefraction = true;
            Vector3 lEndRaycastPosition = Vector3.forward * mMaxDistance;
            RaycastHit lRaycastHit;
            if (Physics.Raycast(new Ray(position, transform.forward),
                out lRaycastHit, mMaxDistance,
                mLayerMask.value))
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
                    case "Button":
                        lRaycastHit.collider.gameObject.GetComponent<PortalButton>().PlayEvent();
                        break;
                }
            }

            mLineRenderer.SetPosition(0, position);
            mLineRenderer.SetPosition(1, lEndRaycastPosition);
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
        else
            mMirrorPortal.CreateRefraction(
                mMirrorPortal.transform.TransformPoint(transform.InverseTransformPoint(position)), true);
    }
}