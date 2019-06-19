using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    public static SceneTransition instance = null;

    public Image overlayImage;
    public float fadeTime = 1.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static void ChangeToScene(int index)
    {
        if (instance == null)
            SceneManager.LoadScene(index);
        else
            instance.StartFade(index);
    }

    public void StartFade(int index)
    {
        StartCoroutine(FadeAnimation(index));
    }

    IEnumerator FadeAnimation(int index)
    {
        overlayImage.gameObject.SetActive(true);
        overlayImage.enabled = true;

        Color overlayColor = overlayImage.color;

        // make copies of overlaycolor with desired alphas to use in the lerp
        Color transparent = overlayColor;
        transparent.a = 0.0f;
        Color full = overlayColor;
        full.a = 1.0f;

        // fade in
        for (float t = 0.0f; t < fadeTime; t += Time.fixedUnscaledDeltaTime)
        {
            overlayImage.color = Color.Lerp(transparent, full, t / fadeTime);
            yield return new WaitForFixedUpdate();
        }

        // chance scene
        SceneManager.LoadScene(index);

        // fade out
        for (float t = 0.0f; t < fadeTime; t += Time.fixedUnscaledDeltaTime)
        {
            overlayImage.color = Color.Lerp(full, transparent, t / fadeTime);
            yield return new WaitForFixedUpdate();
        }

        overlayImage.gameObject.SetActive(false);
    }

}
