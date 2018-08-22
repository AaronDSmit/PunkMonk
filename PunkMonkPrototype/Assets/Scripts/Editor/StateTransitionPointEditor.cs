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
            GUIStyle stateStyle = new GUIStyle();
            stateStyle.normal.textColor = Color.yellow;
            stateStyle.alignment = TextAnchor.MiddleCenter;
            stateStyle.contentOffset = new Vector2(0, -7);

            GUIStyle indexStyle = new GUIStyle();
            indexStyle.normal.textColor = Color.yellow;
            indexStyle.alignment = TextAnchor.LowerRight;
            indexStyle.contentOffset = new Vector2(12, 3);

            stateStyle.fontSize = 20;

            Handles.Label(stateTransitionPoint.transform.position, StateManager.StateToString(stateTransitionPoint.TargetState)[0].ToString(), stateStyle);
            Handles.Label(stateTransitionPoint.transform.position, stateTransitionPoint.index.ToString(), indexStyle);

            if(stateTransitionPoint.EarthHex)
            {
                Gizmos.color = new Color(1.0f, 0.2f, 0.2f, 1.0f); // brown
                DrawArrow.ForGizmo(stateTransitionPoint.transform.position, stateTransitionPoint.EarthHex.transform.position - stateTransitionPoint.transform.position);
            }

            if (stateTransitionPoint.LightningHex)
            {
                Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 1.0f); // blue
                DrawArrow.ForGizmo(stateTransitionPoint.transform.position, stateTransitionPoint.LightningHex.transform.position - stateTransitionPoint.transform.position);
            }

            if (stateTransitionPoint.CheckPoint)
            {
                Gizmos.color = new Color(0.93f, 0.58f, 0.04f, 1.0f); // yellow
                DrawArrow.ForGizmo(stateTransitionPoint.transform.position, stateTransitionPoint.CheckPoint.transform.position - stateTransitionPoint.transform.position);
            }
        }
    }
}