using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float moveSpeed = 10.0f;

    bool onOculusGo = false;

    private void Awake()
    {
        onOculusGo = OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote);
    }

    private void FixedUpdate()
    {
    }

}
