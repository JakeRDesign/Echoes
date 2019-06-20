using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    public float moveSpeed = 100.0f;

    [Tooltip("Oculus go controller object")]
    public Transform goController;
    [Tooltip("Object representing the head - usually the Oculus' CenterEye")]
    public Transform headTransform;

    [Header("Footstep Sounds")]
    [Tooltip("Seconds between footsteps")]
    public float footstepInterval = 0.5f;
    [Tooltip("List of footstep Audio Clips which are looped through when playing footsteps")]
    public List<AudioClip> footstepSounds;
    [Tooltip("AudioSource where footsteps should come from")]
    public AudioSource footstepSource;
    // timer used to space out footsteps
    private float footstepCounter = 0.0f;
    // index of the next footstep sound to play
    private int footstepIndex = 0;

    [Header("Blood Trails")]
    [Tooltip("List from which a random prefab is chosen when spawning blood trail")]
    public List<GameObject> bloodPrefabs;
    [Tooltip("How far the player needs to move before a new blood trail is spawned")]
    public float trailDistance = 0.3f;
    // player position when the last blood trail was spawned
    private Vector3 lastTrail;

    [Header("Breathing")]
    [Tooltip("Sound to loop when unnoticed")]
    public AudioSource normalBreathing;
    [Tooltip("Sound to play when noticed by an enemy")]
    public AudioSource panickedBreathing;
    // keep track of last panicked breathing sound effect so it doesn't spam
    float lastPanicTime = 0.0f;
    // keep track of the last enemy that noticed us, so we can make sure the
    // panicked breathing sound doesn't spam
    EnemyController lastTargeter = null;

    [Header("Death")]
    [Tooltip("Sound to play when killed")]
    public AudioClip deathSound;
    [Tooltip("Seconds to wait before restarting the scene")]
    public float resetDelay = 4.0f;

    // character controller used to move
    CharacterController controller;

    private void Awake()
    {
        // grab reference to the character controller which we use to move
        controller = GetComponent<CharacterController>();

        // hide the oculus controller hand thing if we're not on the oculus
        goController.gameObject.SetActive(Application.isMobilePlatform);

        // initialise trail spawning position so we don't spawn one instantly
        lastTrail = transform.position;

        // make sure there are no phantom echo effects left over
        EchoTrigger.ClearEcho();
    }

    private void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", transform.position);
        // make sure normal breathing starts after panicked breathing is finished
        if (!panickedBreathing.isPlaying && !normalBreathing.isPlaying)
            normalBreathing.Play();

        // check for pause button
        if (PauseButtonPressed())
            PauseMenu.instance.Toggle(headTransform);

        // handle blood trails
        if (Vector3.Distance(lastTrail, transform.position) >= trailDistance)
            SpawnBloodTrail();

        Vector3 moveInput = GetMovementInput();

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

        // transform input vector to a camera-relative vector
        // (positive Z = moving in forward direction)
        Vector3 movement = Vector3.zero;
        movement += moveInput.x * rgt;
        movement += moveInput.z * fwd;

        // apply movement to characterController
        if (controller.enabled)
            controller.SimpleMove(movement * moveSpeed * Time.deltaTime);

        // handle playing footstep sound effects
        if (moveInput.sqrMagnitude > 0.0f)
        {
            // increment footstep timer when movement input exists
            footstepCounter += Time.deltaTime;

            // footstep should be played!
            while (footstepCounter > footstepInterval)
            {
                // decrement footstep timer so we don't lose the time we would
                // if we set it to 0, keeps footstep times more consistent
                footstepCounter -= footstepInterval;

                // play footstep sound from list
                footstepSource.PlayOneShot(footstepSounds[footstepIndex]);
                // move on to next footstep sound, loop around to 0 when at
                // the end of the list
                footstepIndex = (footstepIndex + 1) % footstepSounds.Count;
            }
        }
    }

    void SpawnBloodTrail()
    {
        // choose random prefab
        GameObject toSpawn = bloodPrefabs[Random.Range(0, bloodPrefabs.Count)];

        // raycast downwards to find ground position
        if (!Physics.Raycast(transform.position, Vector3.down, out var hit))
            return;

        GameObject newTrail = Instantiate(toSpawn, hit.point + Vector3.up*0.05f, transform.rotation);

        // rotate the object based on the floor underneath us
        Transform trailTransform = newTrail.transform;
        trailTransform.rotation = Quaternion.FromToRotation(trailTransform.up, hit.normal) * trailTransform.rotation;

        // lastTrail is the position that the player was at when the last one was spawned
        // kind of a bad name, but I'd rather write this long comment explaining it
        // instead of changing the name because I'm very smart
        lastTrail = transform.position;
    }

    #region Enemy Interaction

    /// <summary>
    /// Called when anything enters the trigger collider which surrounds the player
    /// Used to detect if an enemy has collided with us so we know to die
    /// Trigger collider exists as a workaround to strange collision resolution
    /// from using a CharacterController
    /// </summary>
    /// <param name="other">Collider which entered our trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        // die when an enemy touches us :)
        if (other.CompareTag("Enemy"))
            Die(other.GetComponent<EnemyController>());
    }

    /// <summary>
    /// Handles death-related things such as playing death sound
    /// Called when player is killed
    /// </summary>
    /// <param name="killedBy">Enemy which killed us</param>
    public void Die(EnemyController killedBy)
    {
        // no more audio from us, we're dead!
        StopAudioSources(transform);

        // use some audio source to play death sound
        normalBreathing.PlayOneShot(deathSound);

        // disable character controller so we stop moving
        controller.enabled = false;

        // reset the scene, with a delay to wait for the sound effect to finish
        SceneTransition.ChangeToScene(SceneManager.GetActiveScene().buildIndex, resetDelay);
        StartCoroutine(BadHack(killedBy));
    }

    // awful hack to make the enemy get out of your face a little while after the fade starts
    // just move it into the ground after it kills you lmao
    IEnumerator BadHack(EnemyController toMove)
    {
        const float moveTime = 1.0f;

        for(float t = 0.0f; t < moveTime; t += Time.fixedUnscaledDeltaTime)
        {
            toMove.transform.position -= Vector3.up * Time.fixedUnscaledDeltaTime * 1.0f;

            if (t / moveTime > 0.4f)
                toMove.enabled = false;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Recursively stops all audio sources that are children of <paramref name="t"/>
    /// </summary>
    /// <param name="t">Object whose children to check for audio sources</param>
    void StopAudioSources(Transform t)
    {
        // check if this object has an audio source
        AudioSource audio = t.GetComponent<AudioSource>();
        // and stop it if it does
        if (audio != null)
            audio.Stop();

        // call this function on all children :)
        foreach (Transform child in t)
            StopAudioSources(child);
    }

    /// <summary>
    /// Called when an enemy notices the player
    /// Handles playing panicked breathing sounds when noticed
    /// </summary>
    /// <param name="targeter">Enemy which noticed the player</param>
    public void Noticed(EnemyController targeter)
    {
        if (panickedBreathing.isPlaying || (Time.time - lastPanicTime < 5 && targeter == lastTargeter))
            return;

        normalBreathing.Stop();
        panickedBreathing.Play();
        lastPanicTime = Time.time;
        lastTargeter = targeter;
    }

    /// <summary>
    /// Called when an enemy loses sight of the player
    /// </summary>
    public void UnNoticed() { }

    #endregion

    #region Input Stuff

    /// <summary>
    /// Gets a platform-independent vector of movement input
    /// </summary>
    /// <returns>Vector representing the direction the player is inputting</returns>
    Vector3 GetMovementInput()
    {
        // use touchpad on oculus
        if (Application.isMobilePlatform)
        {
            // we only want to be moving while the player is touching the touchpad
            if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
            {
                Vector2 touchPad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                // move in the direction of the touchpad
                return new Vector3(touchPad.x, 0.0f, touchPad.y);
            }
        } // use default unity input on everything else
        else
        {
            return new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Platform-independent check of whether or not the player has 
    /// pressed the pause button
    /// </summary>
    /// <returns>Whether or not the pause button was pressed this frame</returns>
    bool PauseButtonPressed()
    {
        // use back button for oculus
        if (Application.isMobilePlatform)
            return OVRInput.Get(OVRInput.Button.Back);
        else // and tab for testing on PC
            return Input.GetKeyDown(KeyCode.Tab);
    }

    #endregion

}
