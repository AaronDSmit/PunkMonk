using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(SceneTransitionPoint), true)]
[InitializeOnLoad]
public class TransitionPointEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(SceneTransitionPoint transitionPoint, GizmoType gizmoType)
    {
        if (transitionPoint.drawText)
        {
            GUIStyle style = new GUIStyle(); // This is optional
            style.normal.textColor = Color.yellow;
            style.alignment = TextAnchor.MiddleCenter;
            style.contentOffset = new Vector2(0, -7);

            style.fontSize = 20;

            Handles.Label(transitionPoint.transform.position, transitionPoint.NextLevelIndex.ToString(), style);
        }
    }
}