using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu instance = null;
    public float spawnDistance = 2.0f;

    [Header("Animation")]
    public float openTime = 1.0f;
    public float closeTime = 0.3f;

    [Header("Buttons")]
    public List<BoxCollider> buttonColliders;

    bool isOpen = false;

    Vector2 openSize;

    public PauseMenu()
    {
        instance = this;
    }

    private void Awake()
    {
        instance = this;
    }

    public void Toggle(Transform infront)
    {
        if (isOpen)
            Close();
        else
            Open(infront);
    }

    public void Open(Transform inFront)
    {
        transform.position = inFront.position + (inFront.forward * spawnDistance);
        transform.rotation = inFront.rotation;

        this.gameObject.SetActive(true);
        StopAllCoroutines();
        Time.timeScale = 0.0f;
        isOpen = true;

        FindObjectOfType<VRInput>()?.EnableLine();

        StartCoroutine(OpenAnimation());
    }

    public void Close()
    {
        StopAllCoroutines();
        Time.timeScale = 1.0f;
        isOpen = false;

        FindObjectOfType<VRInput>()?.DisableLine();

        StartCoroutine(CloseAnimation());
    }

    IEnumerator OpenAnimation()
    {
        for (float t = 0.0f; t < openTime; t += Time.unscaledDeltaTime)
        {
            float tt = EaseOutElastic(t / openTime);

            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, tt);
            // annoying hack to make raycasts detect the buttons
            foreach (var c in buttonColliders)
            {
                c.enabled = false;
                c.enabled = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CloseAnimation()
    {
        for (float t = 0.0f; t < closeTime; t += Time.unscaledDeltaTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / closeTime);
            yield return new WaitForEndOfFrame();
        }

        this.gameObject.SetActive(false);
    }

    float EaseOutElastic(float t, float magnitude = 0.7f)
    {

        float p = 1.0f - magnitude;
        float scaledTime = t * 2.0f;

        if (t < Mathf.Epsilon || t >= 1.0f - Mathf.Epsilon)
            return t;

        float s = p / (2.0f * Mathf.PI) * Mathf.Asin(1.0f);
        return (
            Mathf.Pow(2.0f, -10.0f * scaledTime) *
            Mathf.Sin((scaledTime - s) * (2 * Mathf.PI) / p)
        ) + 1.0f;

    }

    public void BackToMenu()
    {
        SceneTransition.ChangeToScene(0);
    }

}
