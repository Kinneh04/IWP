using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BlackscreenController : MonoBehaviour
{
    public Image fadePanel; // Reference to the panel you created
    public float fadeSpeed = 1.0f; // Speed of the fade (higher is faster)

    // Static property to hold the single instance of the class
    public static BlackscreenController Instance { get; private set; }
    private void Start()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If an instance already exists, destroy this new instance
            Destroy(gameObject);
            return;
        }

        // Set the instance to this object
        Instance = this;

        // Keep the object alive between scenes (optional)
        fadePanel.gameObject.SetActive(true);
        FadeIn();
    }


    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
       
        Color color = fadePanel.color;
        color.a = 0;
        while (fadePanel.color.a < 1)
        {
            color.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        Color color = fadePanel.color;
        color.a = 1;
        while (fadePanel.color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }
    }
}
