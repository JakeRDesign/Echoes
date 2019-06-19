using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    public float moveSpeed = 100.0f;
    public Transform goController;

    public Transform headTransform;

    [Header("Footstep Sounds")]
    [Tooltip("Seconds between footsteps")]
    public float footstepInterval = 0.5f;
    public List<AudioClip> footstepSounds;
    public AudioSource footstepSource;
    private float footstepCounter = 0.0f;
    private int footstepIndex = 0;

    [Header("Blood Trails")]
    public List<GameObject> bloodPrefabs;
    public float trailDistance = 0.3f;
    private Vector3 lastTrail;

    [Header("Breathing")]
    public AudioSource normalBreathing;
    public AudioSource panickedBreathing;
    float lastPanicTime = 0.0f;
    EnemyController lastTargeter = null;

    CharacterController controller;

    private void Awake()
    {
        goController.gameObject.SetActive(Application.isMobilePlatform);
        controller = GetComponent<CharacterController>();

        lastTrail = transform.position;

        EchoTrigger.ClearEcho();
    }

    private void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", transform.position);
        // make sure normal breathing starts after panicked breathing is finished
        if (!panickedBreathing.isPlaying && !normalBreathing.isPlaying)
            normalBreathing.Play();

        Vector3 moveInput = Vector3.zero;

        // use touchpad on android
        if (Application.isMobilePlatform)
        {
            if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
            {
                Vector2 touchPad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                moveInput = new Vector3(touchPad.x, 0.0f, touchPad.y);
            }

            if (OVRInput.Get(OVRInput.Button.Back))
                PauseMenu.instance.Toggle(headTransform);
        }
        else
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

            if (Input.GetKeyDown(KeyCode.Tab))
                PauseMenu.instance.Toggle(headTransform);
        }

        if (moveInput.sqrMagnitude > 0.0f)
        {
            footstepCounter += Time.deltaTime;

            while (footstepCounter > footstepInterval)
            {
                footstepCounter -= footstepInterval;

                footstepSource.PlayOneShot(footstepSounds[footstepIndex]);

                footstepIndex = (footstepIndex + 1) % footstepSounds.Count;
            }
        }

        if (Vector3.Distance(lastTrail, transform.position) >= trailDistance)
        {
            // choose random prefab
            GameObject toSpawn = bloodPrefabs[Random.Range(0, bloodPrefabs.Count)];
            GameObject newTrail = Instantiate(toSpawn, transform.position - (Vector3.up * 1.3f), transform.rotation);

            lastTrail = transform.position;
        }

        // get direction head is facing
        Vector3 fwd = headTransform.forward;
        // cancel out any vertical movement
        fwd.y = 0.0f;
        // make sure to re-normalize this without any y component
        fwd.Normalize();

        // do the same for right direction
        Vector3 rgt = headTransform.right;
        rgt.y = 0.0f;
        rgt.Normalize();

        Vector3 movement = Vector3.zero;
        movement += moveInput.x * rgt;
        movement += moveInput.z * fwd;

        controller.SimpleMove(movement * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            SceneTransition.ChangeToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Noticed(EnemyController targeter)
    {
        Debug.Log("OwO");

        if (panickedBreathing.isPlaying || (Time.time - lastPanicTime < 5 && targeter == lastTargeter))
            return;

        normalBreathing.Stop();
        panickedBreathing.Play();
        lastPanicTime = Time.time;
        lastTargeter = targeter;
    }

    public void UnNoticed()
    {
        Debug.Log("UnU");
    }

}
