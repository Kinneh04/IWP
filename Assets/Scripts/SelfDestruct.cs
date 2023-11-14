using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float SelfDestructAfterSeconds;
    public GameObject ParticleEffects;
    private void Awake()
    {
        if (ParticleEffects) StartCoroutine(SelfD());
        else Destroy(gameObject, SelfDestructAfterSeconds);
    }



    IEnumerator SelfD()
    {
        yield return new WaitForSeconds(SelfDestructAfterSeconds);
        if (ParticleEffects) Instantiate(ParticleEffects, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
