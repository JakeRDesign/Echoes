using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTrig : MonoBehaviour
{
    public GameObject controls;
    public GameObject text;
    private bool firsttime;

    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "Player") && firsttime == false)
        {
            controls.transform.position = controls.transform.position + new Vector3(0, 10f, 0);
            text.transform.position = controls.transform.position + new Vector3(0, 10f, 0);
            firsttime = true;
        }
    }
}