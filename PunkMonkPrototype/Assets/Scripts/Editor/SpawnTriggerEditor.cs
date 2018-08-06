using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(SpawnTrigger), true)]
[InitializeOnLoad]
public class SpawnTriggerEditor : Editor
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(SpawnTrigger trigger, GizmoType gizmoType)
    {
        GUIStyle style = new GUIStyle();

        style.normal.textColor = Color.magenta;
        style.alignment = TextAnchor.MiddleCenter;
        style.contentOffset = new Vector2(0, -7);
        style.fontSize = 20;

        GUIStyle indexStyle = new GUIStyle();
        indexStyle.normal.textColor = Color.yellow;
        indexStyle.alignment = TextAnchor.UpperLeft;
        indexStyle.contentOffset = new Vector2(12, 3);

        Handles.Label(trigger.transform.position, "T", style);

        Handles.Label(trigger.transform.position, trigger.index.ToString(), indexStyle);

        foreach (Spawner spawner in trigger.spawners)
        {
            DrawArrow.ForGizmo(trigger.transform.position, spawner.transform.position - trigger.transform.position);
        }
    }
}