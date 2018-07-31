using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Base class for any spell that a character can cast:
/// 
/// 1) Object must belong to a Spell Prefab.
/// 2) Object must implement A Cast Spell Function that takes in an array of Tiles and a onStart and OnFinish callback.
///
/// </summary>


public class Spell : ScriptableObject
{
    public virtual void CastSpell(Unit a_unit, Tile[] a_tiles, System.Action a_onStartCB, System.Action a_onFinishCB)
    {
        a_onStartCB();

    }
}