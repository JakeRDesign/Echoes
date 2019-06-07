using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    public float moveSpeed = 100.0f;
    public Transform goController;

    public Transform headTransform;

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
            if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
            {
                Vector2 touchPad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                moveInput = new Vector3(touchPad.x, 0.0f, touchPad.y);
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

        controller.SimpleMove(movement * moveSpeed * Time.deltaTime);
    }

}
