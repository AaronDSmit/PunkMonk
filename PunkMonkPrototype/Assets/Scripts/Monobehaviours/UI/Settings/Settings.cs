using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "settings", menuName = "Settings/new settings", order = 0)]
public class Settings : ScriptableObject
{
    public delegate void OnSettingsChanged();
    public OnSettingsChanged onSettingsChanged = delegate { };

    #region Gameplay

    [HideInInspector]
    [SerializeField]
    public bool inverseCameraRotation = false;
    public bool InverseCameraRotation { set { inverseCameraRotation = value; onSettingsChanged(); } get { return inverseCameraRotation; } }
    [HideInInspector]
    [SerializeField]
    public bool screenEdgePan = true;
    public bool ScreenEdgePan { set { screenEdgePan = value; onSettingsChanged(); } get { return screenEdgePan; } }

    #endregion

    #region Graphics

    [HideInInspector]
    [SerializeField]
    public int quality = 0;
    public int Quality { set { quality = value; onSettingsChanged(); } get { return quality; } }

    #endregion

    #region Sound

    [HideInInspector]
    [SerializeField]
    public bool muteAll = false;
    public bool MuteAll { set { muteAll = value; onSettingsChanged(); } get { return muteAll; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    public float master = 1;
    public float Master { set { master = value; onSettingsChanged(); } get { return master; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    public float music = 1;
    public float Music { set { music = value; onSettingsChanged(); } get { return music; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    public float effects = 1;
    public float Effects { set { effects = value; onSettingsChanged(); } get { return effects; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    public float dialouge = 1;
    public float Dialouge { set { dialouge = value; onSettingsChanged(); } get { return dialouge; } }

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

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/settings.save");
        bf.Serialize(file, this);
        file.Close();

        Debug.Log("Settings saved to: '" + Application.persistentDataPath + "/settings.save" + "'.");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/settings.save"))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            SetAllTo((Settings)bf.Deserialize(file));
            file.Close();

            Debug.Log("Settings Loaded");

        }
        else
        {
            Debug.Log("No Settings Found");
        }
    }
}
