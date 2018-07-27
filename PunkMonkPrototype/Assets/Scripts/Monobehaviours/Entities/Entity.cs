using UnityEngine;

/// <summary>
/// Base class for any game object that can take damage:
/// 
/// 1) Object must belong to a team.
/// 2) Object must implement TakeDamage which takes in an element and optional damage amount.
///
/// </summary>

// used to indicate damage type, even if it deals zero damage
public enum Element { FIRE, EARTH, LIGHTNING, WATER }

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected Team team;

    protected Tile currentTile;

    public Team Team { get { return team; } }

    // damage can be left out if you only want to apply an element.
    public abstract void TakeDamage(Element a_damageType, float a_damageAmount);
}