using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoTrigger : MonoBehaviour
{
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/EchoRidesAgain");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float radius = Mathf.PingPong(Time.time, 30);
            rend.material.SetFloat("_Radius", radius);
        }
    }   
}