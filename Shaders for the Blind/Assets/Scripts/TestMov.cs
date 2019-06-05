using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMov : MonoBehaviour
{
    public float playerSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.forward * playerSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.back * playerSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.left * playerSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.right * playerSpeed * Time.deltaTime;
            }
        }
    }
}
