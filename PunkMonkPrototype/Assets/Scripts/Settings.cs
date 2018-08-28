using UnityEngine;

[CreateAssetMenu(fileName = "settings", menuName = "Settings/new settings", order = 0)]
public class Settings : ScriptableObject
{
    #region Gameplay

    public bool inverseCameraRotation = false;
    public bool screenEdgePan = true;

    #endregion

    #region Sound

    public bool muteAll = false;
    public int master = 100;
    public int music = 100;
    public int effects = 100;
    public int dialouge = 100;

    #endregion

    #region Graphics

    public int quality = 0;


    #endregion

}
