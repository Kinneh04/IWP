using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MunninsTrialManager : MonoBehaviour
{
    public List<GameObject> Dungeons = new List<GameObject>();
    public GameObject BeginningDungeon;
    public GameObject CurrentlyInstantiatedDungeon;
    public bool hasClearedThisDungeon = false;
    public int NumberOfClearedDungeons = 0;
    public List<EnemyScript> EnemiesInDungeon = new List<EnemyScript>();
    public bool isCurrentlyRunningDungeon = false;
    public bool isTransitioningDungeons = false;
    public GameObject DungeonClearIndicator;
    public TMP_Text DungeonsClearedText;
    private static MunninsTrialManager _instance;
    public Transform Player;
    public Transform DungeonTransformParent;
    public bool doneBegin = false;
    public OfficialSongScript MunninsTrialSong;

    public TrialDungeon CurrentTrialDungeon;
    public static MunninsTrialManager Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<MunninsTrialManager>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MunninsTrialManager");
                    _instance = singletonObject.AddComponent<MunninsTrialManager>();
                }
            }

            return _instance;
        }
    }
    public void StartDungeon()
    {
        CurrentlyInstantiatedDungeon = Instantiate(BeginningDungeon);
        Player.position = new Vector3(0, 2, 0);
       // EnemiesInDungeon = EnemySpawner.Instance.InstantiateRandomDungeonEnemies();
        hasClearedThisDungeon = false;
        isCurrentlyRunningDungeon = true;
        CurrentlyInstantiatedDungeon.transform.SetParent(DungeonTransformParent);
    }

    public void TransitionToNewDungeon()
    {
        if (!hasClearedThisDungeon || isTransitioningDungeons) return;
        StartCoroutine(NewDungeonCoroutine());
    }

    private void Update()
    {
        if(isCurrentlyRunningDungeon && !hasClearedThisDungeon)
        {
          if(EnemiesInDungeon.Count <= 0)
            {
                ClearDungeon();
            }
          else
            {
                foreach(EnemyScript ES in EnemiesInDungeon)
                {
                    if(ES == null)
                    {
                        EnemiesInDungeon.Remove(ES);
                    }
                }
            }
        }
    }

    public void ClearDungeon()
    {
        if(!doneBegin)
        {
            doneBegin = true;
            hasClearedThisDungeon = true;
            return;
        }
        CurrentTrialDungeon.Arrow.SetActive(true);
        hasClearedThisDungeon = true;
        DungeonClearIndicator.SetActive(true);
        NumberOfClearedDungeons++;
        DungeonsClearedText.text = NumberOfClearedDungeons.ToString();
        EnemiesInDungeon.Clear();
        
    }

    public IEnumerator NewDungeonCoroutine()
    {

        isTransitioningDungeons = true;
        FirstPersonController.Instance.canMove = false;
        Destroy(CurrentlyInstantiatedDungeon);
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        GameObject TempDungeon = Instantiate(Dungeons[Random.Range(0, Dungeons.Count)]);

        DungeonClearIndicator.SetActive(false); 
        CurrentlyInstantiatedDungeon = TempDungeon;
        CurrentlyInstantiatedDungeon.transform.SetParent(DungeonTransformParent);
        TrialDungeon TD = CurrentlyInstantiatedDungeon.GetComponent<TrialDungeon>();
        CurrentTrialDungeon = TD;
        Player.position = TD.Startposition.position;
        Player.rotation = TD.Startposition.rotation;
        yield return new WaitForSeconds(0.1f);
        TD.ExitDoor.hasHit = false;
        BlackscreenController.Instance.FadeIn();
        FirstPersonController.Instance.canMove = true;
        yield return new WaitForSeconds(0.5f);
        //  EnemiesInDungeon = EnemySpawner.Instance.InstantiateRandomDungeonEnemies();
        EnemiesInDungeon = TD.Enemies;
        hasClearedThisDungeon = false;
        isTransitioningDungeons = false;

    }
}
