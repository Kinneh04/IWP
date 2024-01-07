
using UnityEngine;

public class AttackBeam : MonoBehaviour
{
    float cooldown = 0.0f;
    public int Damage;
    public float interval = 0.5f;
    private void OnTriggerStay(Collider other)
    {
        if (cooldown <= 0 && other.CompareTag("Player"))
        {
            other.GetComponent<FirstPersonController>().TakeDamage(Damage);
            cooldown = interval;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (cooldown <= 0 && other.CompareTag("Player"))
        {
            other.GetComponent<FirstPersonController>().TakeDamage(Damage);
            cooldown = interval;
        }
    }

    private void Update()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }
}
