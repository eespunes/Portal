using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public int mMouseBlueButton;
    public int mMouseOrangeButton;

    public Portal mBluePortal;
    public Portal mOrangePortal;

    public PreviewPortal mBluePortalPreview;
    public PreviewPortal mOrangePortalPreview;

    public LayerMask mShootLayerMask;

    private AudioSource mAudioSource;
    public AudioClip mPortalSound;
    public AudioClip mGravitySound;
    public AudioClip mForceSound;
    public AudioClip mFallSound;
    public float mSetPortalOffset;

    public Image mCrossairImage;

    public Sprite mFullCrossair;
    public Sprite mBlueCrossair;
    public Sprite mOrangeCrossair;
    public Sprite mEmptyCrossair;
    private bool mAttachedObject;
    private bool mAttachingObject;
    private Rigidbody mObjectAttached;
    public Transform mAttachingPosition;
    public float mAttachingObjectSpeed;
    public Quaternion mAttachingObjectStartRotation;

    private float mScale;
    private bool mCreatedPortal;

    void Start()
    {
        mScale = 1;
        mAudioSource = GetComponent<AudioSource>();
        mCrossairImage.sprite = mFullCrossair;
    }

    void Update()
    {
        if (mAttachingObject)
            UpdateAttachedObject();
        else
        {
            if (Input.GetMouseButton(mMouseBlueButton))
            {
                SetPreviewPortal(mBluePortalPreview);
                Scale();
            }

            if (Input.GetMouseButton(mMouseOrangeButton))
            {
                SetPreviewPortal(mOrangePortalPreview);
                Scale();
            }

            if (Input.GetMouseButtonUp(mMouseBlueButton))
            {
                if (mBluePortalPreview.isActiveAndEnabled)
                    SetPortal(mBluePortal, mBluePortalPreview);
            }

            if (Input.GetMouseButtonUp(mMouseOrangeButton))
            {
                if (mOrangePortalPreview.isActiveAndEnabled)
                    SetPortal(mOrangePortal, mOrangePortalPreview);
            }

            ChangeCrossair();
        }
        
        if (Input.GetMouseButtonDown(mMouseBlueButton))
            if (mAttachingObject)
                DetachObject(1000);
            else
                Shoot();

        if (Input.GetMouseButtonDown(mMouseOrangeButton))
            if (mAttachingObject)
                DetachObject(0);
            else
                Shoot();
    }

    private void ChangeCrossair()
    {
        if ((mBluePortal.isActiveAndEnabled || mBluePortalPreview.isActiveAndEnabled) &&
            (mOrangePortal.isActiveAndEnabled || mOrangePortalPreview.isActiveAndEnabled))
            mCrossairImage.sprite = mFullCrossair;
        else if (mBluePortal.isActiveAndEnabled || mBluePortalPreview.isActiveAndEnabled)
            mCrossairImage.sprite = mBlueCrossair;
        else if (mOrangePortal.isActiveAndEnabled || mOrangePortalPreview.isActiveAndEnabled)
            mCrossairImage.sprite = mOrangeCrossair;
        else
            mCrossairImage.sprite = mEmptyCrossair;
    }

    void Shoot()
    {
        Ray lCameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit lRaycastHit;

        if (Physics.Raycast(lCameraRay, out lRaycastHit, 200.0f, mShootLayerMask.value))
            switch (lRaycastHit.collider.tag)
            {
                case "Refraction Companion":
                case "Turret":
                case "Companion":
                    mAudioSource.clip = mGravitySound;
                    mAudioSource.loop = true;
                    mAudioSource.Play();
                    mAttachingObject = true;
                    mObjectAttached = lRaycastHit.collider.gameObject.GetComponent<Rigidbody>();
                    lRaycastHit.collider.gameObject.GetComponent<Collider>().enabled = false;
                    mObjectAttached.GetComponent<Companion>().SetTeleport(false);
                    mObjectAttached.GetComponent<Companion>().mAttached = true;
                    break;
            }
    }

    void UpdateAttachedObject()
    {
        Vector3 lEulerAngles = mAttachingPosition.rotation.eulerAngles;
        if (!mAttachedObject)
        {
            Vector3 lDirection = mAttachingPosition.transform.position - mObjectAttached.transform.position;
            float lDistance = lDirection.magnitude;
            float lMovement = mAttachingObjectSpeed * Time.deltaTime;
            mObjectAttached.isKinematic = true;
            if (lMovement >= lDistance)
            {
                mAttachedObject = true;
                mObjectAttached.MovePosition(mAttachingPosition.position);
                mObjectAttached.MoveRotation(Quaternion.Euler(0.0f, lEulerAngles.y, lEulerAngles.z));
            }
            else
            {
                lDirection /= lDistance;
                mObjectAttached.MovePosition(mObjectAttached.transform.position + lDirection * lMovement);
                mObjectAttached.MoveRotation(Quaternion.Lerp(mAttachingObjectStartRotation,
                    Quaternion.Euler(0.0f, lEulerAngles.y, lEulerAngles.z), 1.0f -
                                                                            Mathf.Min(lDistance / 1.5f, 1.0f)));
            }
        }
        else
        {
            mObjectAttached.MoveRotation(Quaternion.Euler(0.0f, lEulerAngles.y, lEulerAngles.z));
            mObjectAttached.MovePosition(mAttachingPosition.position);
        }
    }

    public void DetachObject(float force)
    {
        mAudioSource.loop = false;
        if (force > 0)
            mAudioSource.clip = mForceSound;
        else
            mAudioSource.clip = mFallSound;
        mAudioSource.Play();
        
        mAttachedObject = false;
        mAttachingObject = false;
        mObjectAttached.isKinematic = false;
        mObjectAttached.gameObject.GetComponent<Collider>().enabled = true;
        mObjectAttached.GetComponent<Companion>().SetTeleport(true);
        mObjectAttached.GetComponent<Companion>().mAttached = false;
        mObjectAttached.AddForce(mAttachingPosition.forward * force);
        mObjectAttached = null;
    }

    void SetPreviewPortal(PreviewPortal portal)
    {
        Ray lCameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit lRaycastHit;
        if (Physics.Raycast(lCameraRay, out lRaycastHit, 200.0f, mShootLayerMask.value))
        {
            var transform1 = portal.transform;
            transform1.position = lRaycastHit.point + lRaycastHit.normal * mSetPortalOffset;
            transform1.forward = lRaycastHit.normal;
            transform1.localScale = Vector3.one * mScale;

            portal.gameObject.SetActive(portal.isValidPosition());
        }
    }

    private void Scale()
    {
        if (Input.mouseScrollDelta.y > 0 && mScale <= 2)
            mScale += .1f;
        if (Input.mouseScrollDelta.y < 0 && mScale >= .5f)
            mScale -= .1f;
    }

    void SetPortal(Portal portal, PreviewPortal preview)
    {
        mAudioSource.clip=mPortalSound;
        mAudioSource.Play();
        mScale = 1;
        portal.transform.position = preview.transform.position;
        portal.transform.forward = preview.transform.forward;
        portal.transform.localScale = preview.transform.localScale;
        portal.ResetCamera();

        portal.gameObject.SetActive(true);
        preview.gameObject.SetActive(false);
    }
}