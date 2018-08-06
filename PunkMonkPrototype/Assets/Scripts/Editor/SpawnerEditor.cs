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
            indexStyle.alignment = TextAnchor.UpperLeft;
            indexStyle.contentOffset = new Vector2(12, 3);

            GUIStyle turnNumStyle = new GUIStyle();
            turnNumStyle.normal.textColor = Color.white;
            turnNumStyle.alignment = TextAnchor.LowerRight;
            turnNumStyle.contentOffset = new Vector2(-12, -20);

            if (spawner.UntiToSpawn)
            {
                Handles.Label(spawner.transform.position, spawner.UntiToSpawn.name[0].ToString(), style);

                if (spawner.TurnToSpawn > 0)
                {
                    Handles.Label(spawner.transform.position, spawner.TurnToSpawn.ToString(), turnNumStyle);
                }

                Handles.Label(spawner.transform.position, spawner.index.ToString(), indexStyle);

                //if (spawner.triggers != null && spawner.triggers.Count > 0)
                //{
                //    foreach (SpawnTrigger trigger in spawner.triggers)
                //    {
                //        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 1.0f); // white
                //        DrawArrow.ForGizmo(spawner.transform.position, trigger.transform.position - spawner.transform.position);
                //    }
                //}

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