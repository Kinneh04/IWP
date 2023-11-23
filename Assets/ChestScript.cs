using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public List<GameObject> RandomAvailableStuff = new List<GameObject>();
    public GameObject ChosenItem;
    public Transform ChestSpawnPos;
    public AnimationClip ChestOpenClip;
    public Animator ChestAnimator;
    public AudioSource ChestAudioSource;
    public AudioClip ChestOpenAudioClip;
    public AudioClip ChestPopupItemAudioClip;
    public GameObject ChestEffects;
    public AudioClip CollectItemAudioClip;
    GameObject SpawnedItem;
    public bool isOpen = false;
    bool readyForCollection = false;
    public ChestSpawner RelatedChestSpawner;
    public AudioClip SpawnedAC;

    private void Start()
    {
        ChestAudioSource.PlayOneShot(SpawnedAC);
    }

    public void OnTriggerStay(Collider other)
    {
        if(readyForCollection)
        {
            if (other.CompareTag("PlayerHitbox") || other.CompareTag("Player"))
                CollectItem();
        }
    }
    public void SpawnRandomItem()
    {
        GameObject GO = RandomAvailableStuff[Random.Range(0, RandomAvailableStuff.Count)];
        SpawnedItem = Instantiate(GO, ChestSpawnPos.position, GO.transform.rotation);
        ChestAudioSource.PlayOneShot(ChestPopupItemAudioClip);
        readyForCollection = true;
        Instantiate(ChestEffects, ChestSpawnPos.position, Quaternion.identity);
    }

    public void OpenChest()
    {
        StartCoroutine(OpenChestCoroutine());
    }

    public IEnumerator OpenChestCoroutine()
    {
        isOpen = true;
        ChestAnimator.Play(ChestOpenClip.name);
        ChestAudioSource.PlayOneShot(ChestOpenAudioClip);
        yield return new WaitForSeconds(0.5f);
        SpawnRandomItem();

    }

    public void CollectItem()
    {
        ChestAudioSource.PlayOneShot(CollectItemAudioClip);
        SpawnedItem.GetComponent<Potion>().UsePotion();
        RelatedChestSpawner.resetValue();
        Destroy(SpawnedItem);
        Destroy(gameObject, 5);
    }
}
