using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public List<Spawner> spawners;

    [HideInInspector]
    [SerializeField]
    public int index;

    public void UpdateSpawnerList()
    {
        Spawner[] allSpawners = FindObjectsOfType<Spawner>();
        spawners = new List<Spawner>();

        foreach (Spawner spawner in allSpawners)
        {
            if (spawner.index == index)
            {
                spawners.Add(spawner);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            if (other.CompareTag("EarthUnit") || other.CompareTag("LightningUnit"))
            {
                foreach (Spawner spawner in spawners)
                {
                    spawner.SpawnUnit();
                }
            }

            enabled = false;
        }
    }
}