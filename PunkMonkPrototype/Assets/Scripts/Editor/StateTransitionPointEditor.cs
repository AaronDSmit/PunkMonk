using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(StateTransitionPoint), true)]
[InitializeOnLoad]
public class StateTransitionPointEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(StateTransitionPoint stateTransitionPoint, GizmoType gizmoType)
    {
        if (stateTransitionPoint.drawText)
        {
            GUIStyle style = new GUIStyle(); // This is optional
            style.normal.textColor = Color.yellow;
            style.alignment = TextAnchor.MiddleCenter;
            style.contentOffset = new Vector2(0, -7);

            style.fontSize = 20;

            Handles.Label(stateTransitionPoint.transform.position, StateManager.StateToString(stateTransitionPoint.TargetState)[0].ToString(), style);
        }
    }
}