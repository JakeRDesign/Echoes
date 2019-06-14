using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPulse : MonoBehaviour
{

    public float decaySpeed = 0.3f;

    public Material normalMaterial;
    public Material pulseMaterial;

    Renderer rend;

    float pulseAmt = 0.0f;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (pulseAmt > 0.0f)
        {
            pulseAmt -= Time.deltaTime * decaySpeed;
            if (pulseAmt < 0.0f)
            {
                pulseAmt = 0.0f;
                rend.material = normalMaterial;
            }
            else
            {
                rend.material.SetFloat("_PulseAmount", pulseAmt);
            }

        }
    }

    public void Pulse()
    {
        pulseAmt = 1.0f;

        rend.material = pulseMaterial;
    }

}
