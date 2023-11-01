using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public int beatsToexplode = 5;
    public MusicController musicController;
    public float ExplosionRadius;
    public int Damage;
    Intervals I = new Intervals();
    public GameObject ExplosionEffects;
    public void Start()
    {
        musicController = GameObject.FindAnyObjectByType<MusicController>();
        I = new Intervals();
        I._steps = 1;
        I._trigger = new UnityEngine.Events.UnityEvent();
        I._trigger.AddListener(delegate { DecrementBeat(); });
        I._lastInterval = 0;
        musicController._intervals.Add(I);
     
    }

    public void Update()
    {
        
    }
    public void DecrementBeat()
    {
        beatsToexplode--;
        if(beatsToexplode <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        //musicController._intervals.Remove(I);
        Instantiate(ExplosionEffects, transform.position, Quaternion.identity);
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject E in Enemies)
        {
            if(Vector3.Distance(E.transform.position, transform.position) < ExplosionRadius)
            {
                E.GetComponent<EnemyScript>().TakeDamage(Damage);
            }
        }
        Destroy(gameObject, 0.05f);

    }
}
