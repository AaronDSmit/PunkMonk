using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldController : MonoBehaviour
{
    private Unit earthUnit;

    private Unit lightningUnit;

    private bool inOverworld;

    private void Awake()
    {
        StateManager.OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == Game_state.overworld);
    }

    private void Update()
    {
        // Don't update if in any other game state
        if (inOverworld)
        {
            ProcessKeyboardInput();
            ProcessMouseInput();
        }
    }

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {

    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {

    }

    // Initialisation function called when the scene is ready, sets up unit references
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

        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");

        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No lightning unit found!");
        }
    }
}