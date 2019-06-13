using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStuff : MonoBehaviour
{
    public GameObject colliders;
    public GameObject stage;

    public GameObject text;
    public GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        colliders.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            stage.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
            colliders.gameObject.SetActive(true);
            light.gameObject.SetActive(false);
        }
    }
}