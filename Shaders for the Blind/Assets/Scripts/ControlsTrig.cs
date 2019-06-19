using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTrig : MonoBehaviour
{
    public Animator cube;
    private bool firsttime;

    public GameObject text;

    private void Start()
    {
        cube.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Player") && firsttime == false)
        {
            cube.enabled = true;
            firsttime = true;
        }
    }

    void Dis()
    {
        text.gameObject.SetActive(false);
    }
}