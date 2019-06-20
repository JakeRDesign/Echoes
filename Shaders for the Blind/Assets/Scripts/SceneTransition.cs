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

        SceneManager.sceneLoaded += ReAttachCanvas;
    }

    private void ReAttachCanvas(Scene scene, LoadSceneMode loadSceneMode)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public static void ChangeToScene(int index, float pauseTime = 0.0f)
    {
        if (instance == null)
            SceneManager.LoadScene(index);
        else
            instance.StartFade(index, pauseTime);
    }

    public void StartFade(int index, float pauseTime)
    {
        StartCoroutine(FadeAnimation(index, pauseTime));
    }

    IEnumerator FadeAnimation(int index, float pauseTime)
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

        // pause if needed
        for(float t = 0.0f; t < pauseTime; t += Time.fixedUnscaledDeltaTime)
            yield return new WaitForFixedUpdate();

        // change scene
        var loading = SceneManager.LoadSceneAsync(index);
        // wait for it to finish loading, hopefully this stops any lagginess
        while(!loading.isDone)
            yield return new WaitForEndOfFrame();

        // fade out
        for (float t = 0.0f; t < fadeTime; t += Time.fixedUnscaledDeltaTime)
        {
            overlayImage.color = Color.Lerp(full, transparent, t / fadeTime);
            yield return new WaitForFixedUpdate();
        }

        overlayImage.gameObject.SetActive(false);
    }


}
