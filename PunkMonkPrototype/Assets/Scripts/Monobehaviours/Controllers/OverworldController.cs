﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldController : MonoBehaviour
{
    private Unit earthUnit;

    private Unit lightningUnit = null;

    private bool inOverworld;

    [SerializeField]
    private float movementSpeed = 0;

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (a_newstate == GameState.overworld);

        if (inOverworld)
        {
            Init();
        }
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
        if (inOverworld)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 vecBetween;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layerMask = 0;
                layerMask |= (1 << LayerMask.NameToLayer("Ground"));

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    vecBetween = hit.transform.position - earthUnit.transform.position;
                    vecBetween.y = 0;
                    //Debug.DrawLine()

                    if (vecBetween.magnitude > 1)
                    {
                        earthUnit.transform.position += vecBetween.normalized * movementSpeed * Time.deltaTime;
                    }
                }
            }
        }
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