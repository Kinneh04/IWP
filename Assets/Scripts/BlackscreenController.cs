using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BlackscreenController : MonoBehaviour
{
    public Image fadePanel; // Reference to the panel you created
    public float fadeSpeed = 1.0f; // Speed of the fade (higher is faster)

    private void Start()
    {
        fadePanel.gameObject.SetActive(true);
        FadeIn();
    }

    private bool isFading = false;

    public void FadeOut()
    {
        isFading = true;
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        isFading = true;
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Color color = fadePanel.color;
        while (fadePanel.color.a < 1)
        {
            color.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }
        isFading = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        Color color = fadePanel.color;
        while (fadePanel.color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }
        isFading = false;
    }
}
