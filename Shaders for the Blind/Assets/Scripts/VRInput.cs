using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInput : MonoBehaviour
{

    LineRenderer line;
    float defaultLength = 5.0f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public void EnableLine()
    {
        GetComponent<LineRenderer>().enabled = true;
    }

    public void DisableLine()
    {
        GetComponent<LineRenderer>().enabled = false;
    }

    void Update()
    {
        RaycastHit hit;
        line.SetPosition(0, transform.position);
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("UI")))
        {
            Debug.LogWarning("yes we hitted.");
            line.SetPosition(1, hit.point);
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                BaseEventData baseEvent = new BaseEventData(EventSystem.current);
                ExecuteEvents.Execute(hit.collider.gameObject, baseEvent, ExecuteEvents.submitHandler);
            }
        }
        else
        {
            line.SetPosition(1, transform.position + transform.forward * defaultLength);
        }
    }
}
