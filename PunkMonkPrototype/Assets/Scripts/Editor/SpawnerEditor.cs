using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(Spawner), true)]
[InitializeOnLoad]
public class SpawnerEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(Spawner spawner, GizmoType gizmoType)
    {
        if(spawner.drawText)
        {
            GUIStyle style = new GUIStyle(); // This is optional

            style.normal.textColor = spawner.TextColour;
            style.alignment = TextAnchor.MiddleCenter;
            style.contentOffset = new Vector2(0, -7);

            style.fontSize = 20;

            Handles.Label(spawner.transform.position, spawner.TurnToSpawn.ToString(), style);
        }
    }
}