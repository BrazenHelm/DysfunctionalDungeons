using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 focus = Vector3.zero;
    private float distance = 10f;
    private float pitch = 45f * Mathf.Deg2Rad;
    private float yaw = 45f * Mathf.Deg2Rad;

    private const float MIN_DISTANCE = 2f, MAX_DISTANCE = 10f;
    private const float PAN_RATE = 2f;  // world units per second
    //private const float ROT_RATE = 1f;  // radians per second


    private const float ROT_TIME = 0.1f; // seconds
    private float rotTimeElapsed = 0f;
    private float startYaw, targetYaw;
    private const float ROT_LOCK_ANGLE = 45f * Mathf.Deg2Rad;

    private const float PAN_TRANSITION_TIME = 0.1f;
    private float panTimeElapsed = 0f;
    private Vector3 startFocus, targetFocus;
    private bool doingPanTransition = false;

    private Vector3 forward = Vector3.zero;
    private Vector3 right = Vector3.zero;

    private bool needsUpdate = false;
    private Mover moverLockedTo = null;


    private void Start()
    {
        focus = new Vector3(5f, 0f, 0f);
        targetYaw = yaw;
        UpdateCameraVectors();
        UpdateCameraPosition();
    }


    private void Update()
    {
        DoCameraPan();
        //DoCameraRotation();
        DoCameraZoom();

        if (needsUpdate)
            UpdateCameraPosition();
    }


    public void Follow(Mover mover)
    {
        moverLockedTo = mover;
        targetFocus = mover.transform.position;
        if (focus != targetFocus)
        {
            startFocus = focus;
            panTimeElapsed = 0f;
            doingPanTransition = true;
        }
    }


    public void StopFollowing()
    {
        moverLockedTo = null;
    }


    public void PanTransitionTo(Vector3 target)
    {
        startFocus = focus;
        targetFocus = target;
        panTimeElapsed = 0f;
        doingPanTransition = true;
    }


    private void UpdateCameraVectors()
    {
        forward.x = Mathf.Cos(yaw);
        forward.z = Mathf.Sin(yaw);
        right.x = Mathf.Sin(yaw);
        right.z = -Mathf.Cos(yaw);
    }


    private void UpdateCameraPosition()
    {
        Vector3 offset = -forward * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch);
        transform.position = focus + offset * distance;
        transform.LookAt(focus);
        needsUpdate = false;
    }


    private void DoCameraPan()
    {
        if (doingPanTransition)
        {
            if (moverLockedTo != null)
            {
                targetFocus = moverLockedTo.transform.position;
            }

            panTimeElapsed += Time.deltaTime;
            float t = panTimeElapsed / PAN_TRANSITION_TIME;

            if (t >= 1)
            {
                focus = targetFocus;
                doingPanTransition = false;
            }
            else focus = targetFocus * t + startFocus * (1f - t);

            needsUpdate = true;
        }
        else if (moverLockedTo != null)
        {
            focus = moverLockedTo.transform.position;
            needsUpdate = true;
        }
        else
        {
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) movement += forward;
            if (Input.GetKey(KeyCode.A)) movement -= right;
            if (Input.GetKey(KeyCode.S)) movement -= forward;
            if (Input.GetKey(KeyCode.D)) movement += right;

            if (movement == Vector3.zero) return;

            focus += movement.normalized * PAN_RATE * Time.deltaTime;
            needsUpdate = true;
        }
    }


    private void DoCameraRotation()
    {
        if (yaw != targetYaw)
        {
            rotTimeElapsed += Time.deltaTime;
            float t = rotTimeElapsed / ROT_TIME;

            if (t >= 1) yaw = targetYaw;
            else yaw = targetYaw * t + startYaw * (1f - t);

            UpdateCameraVectors();
            needsUpdate = true;
        }
        else
        {
            float spinDir = 0f;

            if (Input.GetKeyDown(KeyCode.Q)) spinDir -= 1f;
            if (Input.GetKeyDown(KeyCode.E)) spinDir += 1f;

            if (spinDir == 0f) return;

            startYaw = yaw;
            targetYaw = yaw + spinDir * ROT_LOCK_ANGLE;
            rotTimeElapsed = 0f;
        }
    }


    private void DoCameraZoom()
    {
        //float dDistance = -Input.mouseScrollDelta.y;

        //if (dDistance == 0f) return;

        //distance += dDistance;
        //distance = Mathf.Clamp(distance, MIN_DISTANCE, MAX_DISTANCE);
        //needsUpdate = true;   
    }



}
