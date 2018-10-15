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
    [Range(0, 100)]
    private float movementSpeed;

    [SerializeField]
    private float distanceToOther;

    #endregion

    #region References

    private CharacterController cc = null;

    #endregion

    #region Local Fields

    //private Unit earthUnit;
    private bool inOverworld;
    private Vector3 vecBetween;
    private Queue<Vector3> nodes = new Queue<Vector3>();
    private Unit otherUnit;

    public Queue<Vector3> Nodes
    {
        get
        {
            return nodes;
        }
    }

    public Unit OtherUnit
    {
        set
        {
            otherUnit = value;
        }
    }
    #endregion

    #region Public Methods

    public void Init()
    {

        cc = GetComponent<CharacterController>();


        if (GetComponent<EarthUnit>() == null)
        {
            otherUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        }
        else
        {
            otherUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<Unit>();
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
        cc = GetComponent<CharacterController>();

    }

    private void OnDestroy()
    {
        Manager.instance.StateController.OnGameStateChanged -= GameStateChanged;
    }

    private void Update()
    {
        if (inOverworld == true)
        {
            Vector3 vecBetween = otherUnit.transform.position - transform.position;

            cc.Move(vecBetween.normalized * ((vecBetween.magnitude - distanceToOther) * (movementSpeed / 100)));
            transform.rotation = Quaternion.LookRotation(vecBetween.normalized);
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