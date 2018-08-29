using UnityEngine;

[CreateAssetMenu(fileName = "settings", menuName = "Settings/new settings", order = 0)]
public class Settings : ScriptableObject
{
    #region Gameplay

    public bool inverseCameraRotation = false;
    public bool screenEdgePan = true;

    #endregion

    #region Graphics

    public int quality = 0;

    #endregion

    #region Sound

    public bool muteAll = false;
    [Range(0.0f, 1.0f)]
    public float master = 1;
    [Range(0.0f, 1.0f)]
    public float music = 1;
    [Range(0.0f, 1.0f)]
    public float effects = 1;
    [Range(0.0f, 1.0f)]
    public float dialouge = 1;

    #endregion

    public void SetAllTo(Settings a_other)
    {
        SetGameplayTo(a_other);
        SetGraphicsTo(a_other);
        SetSoundsTo(a_other);
    }

    public void SetGameplayTo(Settings a_other)
    {
        // Gameplay
        inverseCameraRotation = a_other.inverseCameraRotation;
        screenEdgePan = a_other.screenEdgePan;
    }

    public void SetGraphicsTo(Settings a_other)
    {
        // Graphics
        quality = a_other.quality;
    }

    public void SetSoundsTo(Settings a_other)
    {
        // Sounds
        muteAll = a_other.muteAll;
        master = a_other.master;
        music = a_other.music;
        effects = a_other.effects;
        dialouge = a_other.dialouge;
    }

}
