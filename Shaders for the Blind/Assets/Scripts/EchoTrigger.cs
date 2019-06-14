using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EchoTrigger : MonoBehaviour
{

    public float echoSpeed = 1.0f;

    public float echoDist = 0.0f;
    float lastDist = 0.0f;

    List<EnemyPulse> pulses;

    private void Awake()
    {
        pulses = new List<EnemyPulse>();

        foreach (var p in FindObjectsOfType<EnemyPulse>())
            pulses.Add(p);
    }

    void Update()
    {
        if (Application.isPlaying)
            echoDist += Time.deltaTime * echoSpeed;

        Shader.SetGlobalFloat("_EchoRadius", echoDist);

        Shader.SetGlobalVector("_EchoCenter", transform.position);

        foreach(var p in pulses)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);

            if(lastDist < dist && echoDist >= dist)
                p.Pulse();
        }

        lastDist = echoDist;
    }

#if UNITY_EDITOR
    [MenuItem("Echo/Clear Echo")]
#endif
    public static void ClearEcho()
    {
        Shader.SetGlobalFloat("_EchoRadius", 0.0f);
        Shader.SetGlobalVector("_EchoCenter", Vector3.zero);
    }

}