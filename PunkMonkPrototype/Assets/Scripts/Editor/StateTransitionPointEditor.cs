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

            if (stateTransitionPoint.EarthHex)
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

            if (stateTransitionPoint.cameraTargetHex)
            {
                Gizmos.color = Color.grey;
                DrawArrow.ForGizmo(stateTransitionPoint.transform.position, stateTransitionPoint.cameraTargetHex.transform.position - stateTransitionPoint.transform.position);
            }

            // Draw Bounds if it is selected
            if (Selection.transforms.Length > 0 && (Selection.transforms[0] == stateTransitionPoint.gameObject.transform.parent || Selection.transforms[0] == stateTransitionPoint.gameObject.transform))
            {
                Gizmos.color = Color.magenta;
                Vector3 centre = Vector3.zero;
                centre.x = (stateTransitionPoint.camBounds.x + stateTransitionPoint.camBounds.y) / 2;
                centre.z = (stateTransitionPoint.camBounds.z + stateTransitionPoint.camBounds.w) / 2;
                Vector3 size = Vector3.zero;
                size.x = (stateTransitionPoint.camBounds.x - stateTransitionPoint.camBounds.y);
                size.y = 5;
                size.z = (stateTransitionPoint.camBounds.z - stateTransitionPoint.camBounds.w);
                Gizmos.DrawWireCube(centre, size);
            }
        }
    }
}