using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoSpawner : MonoBehaviour
{

    public AudioSource pulseAudio;
    public float pulseCooldown = 5.0f;
    public EchoTrigger sourcePrefab;
    EchoTrigger lastSource;

    float lastPulseTime = 0.0f;

    public void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
            SpawnSource();
    }

    public void SpawnSource()
    {
        if (Time.time - lastPulseTime < pulseCooldown)
            return;

        if (lastSource != null)
            Destroy(lastSource.gameObject);

        lastSource = Instantiate(sourcePrefab, transform.position, Quaternion.identity);
        pulseAudio.Play();
        lastPulseTime = Time.time;
    }

}
