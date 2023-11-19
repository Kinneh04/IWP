using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualMetronome : MonoBehaviour
{
    public GameObject Bar;
    public Transform LBarSpawner, RBarSpawner;
    public float MoveInSpeed;
    public Transform Target;
    public RawImage GlowImage;
    public Transform Parent;
    public void SpawnBars()
    {
        GameObject GOL = Instantiate(Bar, LBarSpawner.position, Quaternion.identity);
        GameObject GOR = Instantiate(Bar, RBarSpawner.position, Quaternion.identity);
        TranslateBar TranslateBarL = GOL.GetComponent<TranslateBar>();
        TranslateBar TranslateBarR = GOR.GetComponent<TranslateBar>();
        if(MusicController.Instance.isDrop)
        {
            GOL.GetComponent<Image>().color = Color.red;
            GOR.GetComponent<Image>().color = Color.red;
        }
        TranslateBarL.Target = Target;
        TranslateBarL.speed = MoveInSpeed;
        TranslateBarR.Target = Target;
        TranslateBarR.speed = MoveInSpeed;
        TranslateBarL.relatedMetro = this;
        TranslateBarR.relatedMetro = this;
        GOL.transform.SetParent(Parent);
        GOR.transform.SetParent(Parent);
    }

    public void PulseGlow()
    {
        GlowImage.color = Color.green;
    }

    private void Update()
    {
        if(GlowImage.color.a != 0)
        {
            GlowImage.color = Color.Lerp(GlowImage.color, new Color(0, 0, 0, 0), 3.5f * Time.deltaTime);
        }
    }
}
