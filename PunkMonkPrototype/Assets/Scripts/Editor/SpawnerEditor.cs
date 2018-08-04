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
        if (spawner.drawText)
        {
            GUIStyle style = new GUIStyle(); // This is optional

            style.normal.textColor = spawner.TextColour;
            style.alignment = TextAnchor.MiddleCenter;
            style.contentOffset = new Vector2(0, -7);
            style.fontSize = 20;

            GUIStyle indexStyle = new GUIStyle();
            indexStyle.normal.textColor = Color.yellow;
            indexStyle.alignment = TextAnchor.LowerRight;
            indexStyle.contentOffset = new Vector2(10, 3);

            if (spawner.EntityToSpawn)
            {
                Handles.Label(spawner.transform.position, spawner.EntityToSpawn.name[0].ToString(), style);
                Handles.Label(spawner.transform.position, spawner.TurnToSpawn.ToString(), indexStyle);
            }
            else if (spawner.CompareTag("EarthUnitSpawn"))
            {
                Handles.Label(spawner.transform.position, "E", style);
            }
            else if (spawner.CompareTag("LightningUnitSpawn"))
            {
                Handles.Label(spawner.transform.position, "L", style);
            }
        }
    }
}