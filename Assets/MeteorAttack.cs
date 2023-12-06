using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttack : MonoBehaviour
{

    int secondsBeforeStrike = 3, secondsBeforeDisappear = 3;
    public GameObject Meteor, Warning, Effect;
    public bool canDamage = false;
    Intervals I;
    bool hasCast = false;
    public ParticleSystem PulseParticles;
    public void Start()
    {
        I = new Intervals();
        I._steps = 1/2;
        UnityEngine.Events.UnityEvent newEvent = new UnityEngine.Events.UnityEvent();
        newEvent.AddListener(delegate { Countdown(); });
        I._trigger = newEvent;

        MusicController.Instance._intervals.Add(I);
    }

    public void Countdown()
    {
        if (!hasCast)
        {
            secondsBeforeStrike -= 1;
            if (secondsBeforeStrike <= 0)
            {
                StartCoroutine(CastMeteorEffect());
            }
        }
        else
        {
            PulseParticles.Play();
            secondsBeforeDisappear -= 1;
            canDamage = true;
            if(secondsBeforeDisappear <= 0)
            {
                I.ToBeDeleted = true;
                canDamage = false;
                Destroy(gameObject, 1);
            }
        }
    }

    public IEnumerator CastMeteorEffect()
    {
        hasCast = true;
        Warning.SetActive(false);
        Meteor.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        Meteor.SetActive(false);
        Effect.SetActive(true);
        canDamage = true;
    }
}
