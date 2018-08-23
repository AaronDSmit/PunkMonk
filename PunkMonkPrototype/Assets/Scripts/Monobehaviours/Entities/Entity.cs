using UnityEngine;

/// <summary>
/// Base class for any game object that can take damage:
/// 
/// 1) Object must belong to a team.
/// 2) Object must implement TakeDamage which takes in a damage amount.
///
/// </summary>


public abstract class Entity : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    protected TEAM team;

    #endregion

    #region Local Fields

    protected Hex currentTile;

    #endregion

    #region Properties

    public TEAM Team { get { return team; } }

    public Hex CurrentTile
    {
        get { return currentTile; }
    }

    #endregion

    #region Public Methods

    public abstract void TakeDamage(int a_damageAmount);

    #endregion
}