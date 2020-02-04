using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    float mYaw;
    float mPitch;

    public float mYawRotationalSpeed = 360f;

    public float mPitchRotationalSpeed = 180f;
    public float mMinPitch = -80f;
    public float mMaxPitch = 50f;

    public Transform mPitchControllerTransform;

    private CharacterController mCharacterController;
    public float mSpeed = 10f;

    public KeyCode mLeftKeyCode = KeyCode.A;

    public KeyCode mRightKeyCode = KeyCode.D;

    public KeyCode mForwardKeyCode = KeyCode.W;

    public KeyCode mBackwardsKeyCode = KeyCode.S;

    float mVerticalSpeed;
    bool mOnGround;

    public KeyCode mRunKeyCode = KeyCode.LeftShift;
    public KeyCode mJumpKeyCode = KeyCode.Space;
    public float mFastSpeedMultiplier = 1.2f;
    public float mJumpSpeed = 10f;

    private bool mTeleporting;
    private Vector3 mTeleportingPosition;
    public float mTeleportingOffset;
    private AudioSource mAudioSource;
    public AudioClip mTeleportSound;


    // Start is called before the first frame update
    void Awake()
    {
        mYaw = transform.rotation.eulerAngles.y;
        mPitch = mPitchControllerTransform.localRotation.eulerAngles.x;
        mCharacterController = GetComponent<CharacterController>();
        mAudioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        transform.position = SingletonCheckPoint.getInstance().getPosition();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void LateUpdate()
    {
        if (mTeleporting)
        {
            transform.position = mTeleportingPosition;
            mTeleporting = false;
        }
    }

    private void Movement()
    {
        //Pitch
        float lMouseAxisY = -Input.GetAxis("Mouse Y");
        mPitch += lMouseAxisY * mPitchRotationalSpeed * Time.deltaTime;
        mPitch = Mathf.Clamp(mPitch, mMinPitch, mMaxPitch);
        mPitchControllerTransform.localRotation = Quaternion.Euler(mPitch, 0, 0);


        //Yaw
        float lMouseAxisX = Input.GetAxis("Mouse X");
        mYaw += lMouseAxisX * mYawRotationalSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, mYaw, 0);

        //Movement
        Vector3 lMovement = new Vector3(0, 0, 0);
        float lYawInRadians = mYaw * Mathf.Deg2Rad;
        float lYaw90InRadians = (mYaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 lForward = new Vector3(Mathf.Sin(lYawInRadians), 0.0f, Mathf.Cos(lYawInRadians));
        Vector3 lRight = new Vector3(Mathf.Sin(lYaw90InRadians), 0.0f, Mathf.Cos(lYaw90InRadians));
        if (Input.GetKey(mForwardKeyCode))
            lMovement = lForward;
        else if (Input.GetKey(mBackwardsKeyCode))
            lMovement = -lForward;

        if (Input.GetKey(mRightKeyCode))
            lMovement += lRight;
        else if (Input.GetKey(mLeftKeyCode))
            lMovement -= lRight;

        lMovement.Normalize();
        lMovement = lMovement * Time.deltaTime * mSpeed;

        //Gravity
        mVerticalSpeed += Physics.gravity.y * Time.deltaTime;
        lMovement.y = mVerticalSpeed * Time.deltaTime;

        float lSpeedMultiplier = 1f;
        if (Input.GetKey(mRunKeyCode))
            lSpeedMultiplier = mFastSpeedMultiplier;

        lMovement *= Time.deltaTime * mSpeed * lSpeedMultiplier;

        CollisionFlags lCollisionFlags = mCharacterController.Move(lMovement);
        if ((lCollisionFlags & CollisionFlags.Below) != 0)
        {
            mOnGround = true;
            mVerticalSpeed = 0.0f;
        }
        else
            mOnGround = false;

        if ((lCollisionFlags & CollisionFlags.Above) != 0 && mVerticalSpeed > 0.0f)
            mVerticalSpeed = 0.0f;
        // Jump
        if (mOnGround && Input.GetKeyDown(mJumpKeyCode))
            mVerticalSpeed = mJumpSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "DeadZone":
                SceneManager.LoadScene("GameOver");
                break;
            case "Portal":
                Teleport(other.transform.GetComponent<Portal>());
                break;
        }
    }

    public void Teleport(Portal portal)
    {
        mAudioSource.clip = mTeleportSound;
        mAudioSource.Play();
        Vector3 lPosition = portal.transform.InverseTransformPoint(transform.position);
        mTeleportingPosition = portal.mMirrorPortal.transform.TransformPoint(lPosition);

        Vector3 lDirection = portal.transform.InverseTransformDirection(-transform.forward);
        transform.forward = portal.mMirrorPortal.transform.TransformDirection(lDirection);
        mYaw = transform.rotation.eulerAngles.y;
        mTeleportingPosition += transform.forward * mTeleportingOffset;
        transform.position = mTeleportingPosition;
        mTeleporting = true;
    }
}