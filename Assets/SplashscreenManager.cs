using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SplashscreenManager : MonoBehaviour
{
    public SplashScreenClass[] splashImages; // Array to hold your splash screen images
    public float fadeDuration = 1.0f; // Time taken for fading in/out
    public float displayTime = 2.0f; // Time to display each image
    public Image IconImage;
    public Image BackgroundImage;
    public GameObject LoadingScreen;
    public TMP_Text TitleText;
    public float DropTime;
    public int BPM;
    public float currentTime;
    public List<GameObject> ToPulseAlong = new List<GameObject>();
    public float currentScale = 1.0f;
    bool dropped = false, racked = false;
    public float rackTime = 0.75f;
    public AudioClip RackingShotgun, ShootingGun;
    public AudioSource SplashscreenAS;
    private void Update()
    {
        DropTime -= Time.deltaTime;
       
        if (DropTime <= 0 && !dropped)
        {
            SplashscreenAS.PlayOneShot(ShootingGun);
            DROPSplashscreen();
            dropped = true;
        }
        if (DropTime < rackTime && !racked)
        {
            racked = true;
            SplashscreenAS.PlayOneShot(RackingShotgun);
        }
            currentTime += Time.deltaTime;
        if(currentTime > 60 / (float)BPM)
        {
            currentScale = 1.05f;
            currentTime = 0;
        }
        if(currentScale > 1.0f)
        {
            currentScale = Mathf.Lerp(currentScale, 1.0f, 1 * Time.deltaTime);
            foreach(GameObject obj in ToPulseAlong)
            {
                obj.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
        }
    }

    IEnumerator DropSplashscreenCoroutine()
    {
        BackgroundImage.color = Color.white;
        float t = 1.0f;
        BackgroundImage.raycastTarget = false;
        while(t > 0f)
        {
            yield return null;
            t = Mathf.Lerp(t, 0f, 2 * Time.deltaTime);
            BackgroundImage.color = new Color(1, 1, 1, t);
        }
        BackgroundImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(this);
    }

    public void DROPSplashscreen()
    {
        StartCoroutine(DropSplashscreenCoroutine());
    }



    void Start()
    {
        StartCoroutine(DisplayImages());
    }

    IEnumerator DisplayImages()
    {
        foreach (SplashScreenClass SSC in splashImages)
        {
            yield return StartCoroutine(FadeIn(SSC.image, SSC.Text));
            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn(Sprite image, string text)
    {
        float elapsedTime = 0;
        IconImage.sprite = image;
        IconImage.gameObject.SetActive(true);
        TitleText.text = text;
        TitleText.gameObject.SetActive(true);
        float t = 0;
        while (elapsedTime < fadeDuration)
        {
            t = Mathf.Lerp(0, 1, (elapsedTime / fadeDuration));
            TitleText.color = new Color(1, 1, 1, t);
            IconImage.color = new Color(1, 1, 1, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float t = 1;
        while (elapsedTime < fadeDuration)
        {
            t = Mathf.Lerp(1, 0, (elapsedTime / fadeDuration));
            IconImage.color = new Color(1, 1, 1, t);
            TitleText.color = new Color(1, 1, 1, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        TitleText.gameObject.SetActive(false);
        IconImage.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class SplashScreenClass
{
    public Sprite image;
    public string Text;
}
