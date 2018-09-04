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

    //private Unit earthUnit;
    private bool inOverworld;
    private Vector3 vecBetween;
    private Queue<Vector3> nodes = new Queue<Vector3>();

    public Queue<Vector3> Nodes
    {
        get
        {
            return nodes;
        }
    }
    #endregion

    #region Public Methods

    public void Init()
    {
        //GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        //if (earthGO)
        //{
        //    earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        //}
        //else
        //{
        //    Debug.LogError("No Earth unit found!");
        //}
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void Update()
    {
        if (inOverworld && nodes.Count > 1)
        {
            vecBetween = nodes.Peek() - transform.position;
            transform.position += vecBetween.normalized * movementSpeed * Mathf.Clamp(nodes.Count, 1, 3) * Time.deltaTime;

            if (Vector3.Distance(transform.position, nodes.Peek()) < 0.1)
            {
                nodes.Dequeue();
            }
        }
    }

    #endregion

    #region Local Methods

    private void GameStateChanged(GameState _oldstate, GameState _newstate)
    {
        nodes.Clear();

        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == GameState.overworld);
    }

    #endregion
}