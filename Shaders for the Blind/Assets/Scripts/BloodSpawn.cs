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

        if (Input.GetKey(KeyCode.S))
        {
            moving = true;
            transform.position = transform.position + new Vector3(-.1f, 0, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            moving = true;
            transform.position = transform.position + new Vector3(0, 0, .1f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moving = true;
            transform.position = transform.position + new Vector3(0, 0, -.1f);
        }

        if (moving == true)
        {
            Instantiate(blood[Random.Range(0, blood.Length)], player.position, player.rotation);
            new WaitForSeconds(2f);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            moving = false;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            moving = false;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            moving = false;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            moving = false;
        }
    }
}
