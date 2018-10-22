using UnityEngine;
using UnityEditorInternal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(GameSettings))]
public class SetingsEditor : Editor
{
    // Gameplay
    private SerializedProperty inverseCameraRotation;
    private SerializedProperty screenEdgePan;

    // Graphics
    private SerializedProperty quality;

    // Sound
    private SerializedProperty muteAll;
    private SerializedProperty master;
    private SerializedProperty music;
    private SerializedProperty effects;
    private SerializedProperty dialouge;


    private void OnEnable()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        // Gameplay
        inverseCameraRotation = serializedObject.FindProperty("inverseCameraRotation");
        screenEdgePan = serializedObject.FindProperty("screenEdgePan");

        // Graphics
        quality = serializedObject.FindProperty("quality");

        // Sound
        muteAll = serializedObject.FindProperty("muteAll");
        master = serializedObject.FindProperty("master");
        music = serializedObject.FindProperty("music");
        effects = serializedObject.FindProperty("effects");
        dialouge = serializedObject.FindProperty("dialouge");
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(130) };
            GUILayout.BeginVertical();
            {
                // Gameplay
                GUILayout.Label("Gameplay", EditorStyles.boldLabel, options);
                GUILayout.Label(" Inverse Camera Rotation", options);
                GUILayout.Label(" Screen Edge Pan", options);
                // Grahpics
                GUILayout.Label("Grahpics", EditorStyles.boldLabel, options);
                GUILayout.Label(" Quality", options);
                // Sound
                GUILayout.Label("Sound", EditorStyles.boldLabel, options);
                GUILayout.Label(" Mute All", options);
                GUILayout.Label(" Master", options);
                GUILayout.Label(" Music", options);
                GUILayout.Label(" Effects", options);
                GUILayout.Label(" Dialouge", options);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                // Gameplay
                GUILayout.Label("", EditorStyles.boldLabel);
                inverseCameraRotation.boolValue = EditorGUILayout.Toggle(inverseCameraRotation.boolValue);
                screenEdgePan.boolValue = EditorGUILayout.Toggle(screenEdgePan.boolValue);
                // Grahpics
                GUILayout.Label("", EditorStyles.boldLabel);
                quality.intValue = EditorGUILayout.IntSlider(quality.intValue, 0, 6);
                // Sound
                GUILayout.Label("", EditorStyles.boldLabel);
                muteAll.boolValue = EditorGUILayout.Toggle(muteAll.boolValue);
                master.floatValue = EditorGUILayout.Slider(master.floatValue, 0.0f, 1.0f);
                music.floatValue = EditorGUILayout.Slider(music.floatValue, 0.0f, 1.0f);
                effects.floatValue = EditorGUILayout.Slider(effects.floatValue, 0.0f, 1.0f);
                dialouge.floatValue = EditorGUILayout.Slider(dialouge.floatValue, 0.0f, 1.0f);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        // Always apply the values after changing them
        serializedObject.ApplyModifiedProperties();
    }
}
