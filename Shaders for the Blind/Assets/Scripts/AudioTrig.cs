using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrig : MonoBehaviour
{
    public AudioSource screech;
    private bool firsttime = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (firsttime == false)
            {
                screech.Play();
                firsttime = true;
            }
        }
    }
}
