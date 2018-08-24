using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class OverworldFollower : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private float movementSpeed;

    #endregion

    #region Local Fields

    private Unit earthUnit;
    private bool inOverworld;
    private Vector3 vecBetween;

    #endregion

    #region Public Methods

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

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void Update()
    {
        if (inOverworld && earthUnit)
        {
            if (Vector3.Distance(transform.position, earthUnit.transform.position) > 2)
            {
                vecBetween = earthUnit.transform.position - transform.position;
                transform.position += vecBetween.normalized * movementSpeed * Time.deltaTime;
            }
            else if(Vector3.Distance(transform.position, earthUnit.transform.position) < 1) // push away if too close
            {
                vecBetween = transform.position - earthUnit.transform.position;
                transform.position += vecBetween.normalized * movementSpeed * Time.deltaTime;
            }
        }
    }

    #endregion

    #region Local Methods

    private void GameStateChanged(GameState _oldstate, GameState _newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == GameState.overworld);
    }

    #endregion
}