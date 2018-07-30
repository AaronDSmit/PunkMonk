using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldFollower : MonoBehaviour
{

    private Unit earthUnit;
    private bool inOverworld;
    private Vector3 vecBetween;

    [SerializeField] private float movementSpeed;
    
    private void Awake()
    {
        StateManager.OnGameStateChanged += GameStateChanged;
    }

    public void Init()
    {
        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == Game_state.overworld);
    }

    private void Update()
    {
        if (inOverworld)
        {
            if (Vector3.Distance(transform.position, earthUnit.transform.position) > 2)
            {
                vecBetween = earthUnit.transform.position - transform.position;
                transform.position += vecBetween.normalized * movementSpeed * Time.deltaTime;
            }
        }
    }

}
