using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePlatform : MovingPlatform
{
    public Vector3 mFirstPosition;
    public Vector3 mSecondPosition;
    private bool mMovingToFirst;

    // Update is called once per frame
    void Update()
    {
        if (mMovingToFirst)
        {
            if (transform.position != mFirstPosition)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, mFirstPosition, Time.deltaTime);
            }
            else
            {
                mMovingToFirst = false;
            }
        }
        else
        {
            if (transform.position != mSecondPosition)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, mSecondPosition, Time.deltaTime);
            }
            else
            {
                mMovingToFirst = true;
            }
        }
    }
}