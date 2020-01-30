using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MovingPlatform
{
    public Vector3 mFirstPosition;
    public Vector3 mSecondPosition;
    private bool mMovingToFirst;

    // Update is called once per frame
    void Update()
    {
        if (!mMoving) return;
        if (mMovingToFirst)
        {
            if (transform.position != mFirstPosition)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, mFirstPosition, 2.5f*Time.deltaTime);
            }
            else
            {
                mMovingToFirst = false;
                mMoving = false;
            }
        }
        else
        {
            if (transform.position != mSecondPosition)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, mSecondPosition, 2.5f*Time.deltaTime);
            }
            else
            {
                mMovingToFirst = true;
                mMoving = false;
                SceneManager.LoadScene("Win");
            }
        }
    }
}