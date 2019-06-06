using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpawn : MonoBehaviour
{
    public GameObject[] blood;
    private bool moving = false;

    public Transform player;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            moving = true;
            transform.position = transform.position + new Vector3(.1f, 0, 0);
        }

        if (moving == true)
        {
            Instantiate(blood[Random.Range(0, blood.Length)], player.position, player.rotation);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            moving = false;
        }
    }
}
