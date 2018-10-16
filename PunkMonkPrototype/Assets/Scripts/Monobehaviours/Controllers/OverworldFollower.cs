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
    private float closeDistanceToOther = 2f;

    [SerializeField]
    private float farDistanceToOther = 3f;

    #endregion

    #region References

    private CharacterController cc = null;
    private Animator animator = null;

    #endregion

    #region Local Fields

    //private Unit earthUnit;
    private bool inOverworld;
    private Unit otherUnit;

    private bool approaching = false;

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
            otherUnit = Manager.instance.PlayerController.EarthUnit;
        }
        else
        {
            otherUnit = Manager.instance.PlayerController.LightningUnit;
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
        cc = GetComponent<CharacterController>();
        animator = transform.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No Animator in the children of '" + gameObject.name + "'.", gameObject);
        }
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
            if (animator)
                animator.SetBool("Running", approaching);


            if (approaching)
            {
                cc.Move(vecBetween.normalized * movementSpeed * Time.deltaTime);

                if (vecBetween.magnitude < closeDistanceToOther)
                {
                    approaching = false;
                }
            }
            else
            {
                if (vecBetween.magnitude > farDistanceToOther)
                {
                    approaching = true;
                }
            }

            if (vecBetween.sqrMagnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(vecBetween.normalized);
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