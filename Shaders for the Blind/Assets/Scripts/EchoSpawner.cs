using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoSpawner : MonoBehaviour
{

    public EchoTrigger sourcePrefab;
    EchoTrigger lastSource;

    public void FixedUpdate()
    {
        if(Input.anyKeyDown)
        {
            SpawnSource();
        }
    }

    void SpawnSource()
    {
        if(lastSource != null)
            Destroy(lastSource.gameObject);

        lastSource = Instantiate(sourcePrefab, transform.position, Quaternion.identity);
    }

}
