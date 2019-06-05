using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EchoTrigger : MonoBehaviour
{

    public float echoSpeed = 1.0f;

    public float echoDist = 0.0f;

    void Update()
    {
        if (Application.isPlaying)
            echoDist += Time.deltaTime * echoSpeed;

        Shader.SetGlobalFloat("_EchoRadius", echoDist);

        Shader.SetGlobalVector("_EchoCenter", transform.position);
    }

#if UNITY_EDITOR
    [MenuItem("Echo/Clear Echo")]
    public static void ClearEcho()
    {
        Shader.SetGlobalFloat("_EchoRadius", 0.0f);
        Shader.SetGlobalVector("_EchoCenter", Vector3.zero);
    }
#endif

}