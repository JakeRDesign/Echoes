﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveMode
{
    DRAG = 0,
    JOYSTICK,

    COUNT
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    public float moveSpeed = 100.0f;
    public Transform goController;
    public MoveMode moveMode;

    public Transform headTransform;

    bool isSwiping = false;
    Vector2 swipeStart;

    CharacterController controller;

    private void Awake()
    {
        goController.gameObject.SetActive(Application.isMobilePlatform);
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 moveInput = Vector3.zero;

        // use touchpad on android
        if (Application.isMobilePlatform)
        {
            if(OVRInput.Get(OVRInput.Button.Back))
            {
                moveMode = (MoveMode)(((int)moveMode + 1) % (int)MoveMode.COUNT);
                FindObjectOfType<EchoSpawner>().SpawnSource();
            }

            if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
            {
                Vector2 touchPad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                if (!isSwiping)
                {
                    isSwiping = true;
                    swipeStart = touchPad;
                }
                else
                {
                    Vector2 dif = swipeStart - touchPad;
                    if (moveMode == MoveMode.JOYSTICK)
                        dif = touchPad - swipeStart;
                    if (dif.sqrMagnitude > 0.001f)
                    {
                        moveInput = new Vector3(dif.x, 0.0f, dif.y);
                        if (moveMode == MoveMode.DRAG)
                            swipeStart = touchPad;
                    }
                }
            }
            else
            {
                isSwiping = false;
            }
        }
        else
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        }

        // get direction head is facing
        Vector3 fwd = headTransform.forward;
        // cancel out any vertical movement
        fwd.y = 0.0f;
        // make sure to re-normalize this without any y component
        fwd.Normalize();

        // do the same for right direction
        Vector3 rgt = headTransform.right;
        rgt.y = 0.0f;
        rgt.Normalize();

        Vector3 movement = Vector3.zero;
        movement += moveInput.x * rgt;
        movement += moveInput.z * fwd;

        float mspd = moveMode == MoveMode.JOYSTICK ? moveSpeed * 0.1f : moveSpeed;
        //transform.position += movement * mspd * Time.deltaTime;
        controller.SimpleMove(movement * mspd * Time.deltaTime);
    }

}
