using UnityEngine;
using UnityEditorInternal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(InteractionRuleset))]
public class InteractionRulesetEditor : Editor
{
    private SerializedProperty actionType;

    private SerializedProperty interactableLayers;

    private SerializedProperty WithinRangeHighlightColour;

    private SerializedProperty ValidHighlightColour;

    private SerializedProperty InvalidHighlightColour;

    private SerializedProperty useDistanceCheck;

    private SerializedProperty useTeamCheck;

    private SerializedProperty useTileOccupation;

    private SerializedProperty distanceCheckType;

    private SerializedProperty distanceCheckLength;

    private SerializedProperty maxDistance;

    private SerializedProperty targetTeam;

    private SerializedProperty targetOccupation;

    private void OnEnable()
    {
        interactableLayers = serializedObject.FindProperty("interactableLayers");

        WithinRangeHighlightColour = serializedObject.FindProperty("withinRangeHighlightColour");

        ValidHighlightColour = serializedObject.FindProperty("validHighlightColour");

        InvalidHighlightColour = serializedObject.FindProperty("invalidHighlightColour");

        actionType = serializedObject.FindProperty("actionType");

        useDistanceCheck = serializedObject.FindProperty("useDistanceCheck");

        useTeamCheck = serializedObject.FindProperty("useTeamCheck");

        useTileOccupation = serializedObject.FindProperty("useTileOccupationCheck");

        distanceCheckType = serializedObject.FindProperty("distanceCheckType");

        distanceCheckLength = serializedObject.FindProperty("distanceType");

        maxDistance = serializedObject.FindProperty("minDistance");

        targetTeam = serializedObject.FindProperty("targetTeam");

        targetOccupation = serializedObject.FindProperty("targetOccupation");
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Action Type: ");
        ActionType action = (ActionType)EditorGUILayout.EnumPopup("", (ActionType)actionType.enumValueIndex);
        actionType.enumValueIndex = (int)action;

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Target Layers: ");
        LayerMask tempMask = EditorGUILayout.MaskField(InternalEditorUtility.LayerMaskToConcatenatedLayersMask(interactableLayers.intValue), InternalEditorUtility.layers);
        interactableLayers.intValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Within Range Colour: ");

        GUILayout.FlexibleSpace();

        WithinRangeHighlightColour.colorValue = EditorGUILayout.ColorField(WithinRangeHighlightColour.colorValue);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Valid Colour: ");

        GUILayout.FlexibleSpace();

        ValidHighlightColour.colorValue = EditorGUILayout.ColorField(ValidHighlightColour.colorValue);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Invalid Colour: ");

        GUILayout.FlexibleSpace();

        InvalidHighlightColour.colorValue = EditorGUILayout.ColorField(InvalidHighlightColour.colorValue);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Distance Check: ");

        GUILayout.FlexibleSpace();

        useDistanceCheck.boolValue = EditorGUILayout.Toggle(useDistanceCheck.boolValue);

        GUILayout.EndHorizontal();

        if (useDistanceCheck.boolValue == true)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Distance Formulae: ");

            DistanceType distanceLength = (DistanceType)EditorGUILayout.EnumPopup("", (DistanceType)distanceCheckLength.enumValueIndex);
            distanceCheckLength.enumValueIndex = (int)distanceLength;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Use Unit's : ");

            DistanceCheck distanceType = (DistanceCheck)EditorGUILayout.EnumPopup("", (DistanceCheck)distanceCheckType.enumValueIndex);
            distanceCheckType.enumValueIndex = (int)distanceType;

            GUILayout.EndHorizontal();

            if (distanceType == DistanceCheck.custom)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Maximum Distance: ");

                GUILayout.FlexibleSpace();

                maxDistance.intValue = EditorGUILayout.IntField(maxDistance.intValue);

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label("Team Check: ");

        GUILayout.FlexibleSpace();

        useTeamCheck.boolValue = EditorGUILayout.Toggle(useTeamCheck.boolValue);

        GUILayout.EndHorizontal();

        if (useTeamCheck.boolValue == true)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Target Team: ");

            GUILayout.FlexibleSpace();

            TargetTeam team = (TargetTeam)EditorGUILayout.EnumPopup((TargetTeam)targetTeam.enumValueIndex);
            targetTeam.enumValueIndex = (int)team;

            GUILayout.EndHorizontal();
        }

        if (tempMask.value == 32)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Tile Occupation Check: ");

            GUILayout.FlexibleSpace();

            useTileOccupation.boolValue = EditorGUILayout.Toggle(useTileOccupation.boolValue);

            GUILayout.EndHorizontal();

            if (useTileOccupation.boolValue == true)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Tile state: ");

                GUILayout.FlexibleSpace();

                TileOccupation occupation = (TileOccupation)EditorGUILayout.EnumPopup((TileOccupation)targetOccupation.enumValueIndex);
                targetOccupation.enumValueIndex = (int)occupation;

                GUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
