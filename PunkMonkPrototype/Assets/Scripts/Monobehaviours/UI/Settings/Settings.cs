using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Settings
{
    public delegate void OnSettingsChanged();
    public OnSettingsChanged onSettingsChanged = delegate { };

    [HideInInspector]
    [SerializeField]
    private string name = "newSettings";
    public string Name { set { name = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return name; } }

    #region Gameplay

    [HideInInspector]
    [SerializeField]
    private bool inverseCameraRotation = false;
    public bool InverseCameraRotation { set { inverseCameraRotation = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return inverseCameraRotation; } }
    [HideInInspector]
    [SerializeField]
    private bool screenEdgePan = true;
    public bool ScreenEdgePan { set { screenEdgePan = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return screenEdgePan; } }

    #endregion

    #region Graphics

    [HideInInspector]
    [SerializeField]
    private int quality = 0;
    public int Quality { set { quality = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return quality; } }

    #endregion

    #region Sound

    [HideInInspector]
    [SerializeField]
    private bool muteAll = false;
    public bool MuteAll { set { muteAll = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return muteAll; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    private float master = 1;
    public float Master { set { master = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return master; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    private float music = 1;
    public float Music { set { music = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return music; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    private float effects = 1;
    public float Effects { set { effects = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return effects; } }
    [Range(0.0f, 1.0f)]
    [HideInInspector]
    [SerializeField]
    private float dialouge = 1;
    public float Dialouge { set { dialouge = value; if (onSettingsChanged != null) onSettingsChanged(); } get { return dialouge; } }

    #endregion

    public void SetAllTo(Settings a_other)
    {
        SetGameplayTo(a_other);
        SetGraphicsTo(a_other);
        SetSoundsTo(a_other);

        if (onSettingsChanged != null)
            onSettingsChanged();
    }

    public void SetGameplayTo(Settings a_other)
    {
        // Gameplay
        inverseCameraRotation = a_other.inverseCameraRotation;
        screenEdgePan = a_other.screenEdgePan;

        if (onSettingsChanged != null)
            onSettingsChanged();
    }

    public void SetGraphicsTo(Settings a_other)
    {
        // Graphics
        quality = a_other.quality;

        if (onSettingsChanged != null)
            onSettingsChanged();
    }

    public void SetSoundsTo(Settings a_other)
    {
        // Sounds
        muteAll = a_other.muteAll;
        master = a_other.master;
        music = a_other.music;
        effects = a_other.effects;
        dialouge = a_other.dialouge;

        if (onSettingsChanged != null)
            onSettingsChanged();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + name + ".settings");
        bf.Serialize(file, this);
        file.Close();

        Debug.Log("Settings saved to: '" + Application.persistentDataPath + "/" + name + ".settings" + "'.");
    }

    public static Settings Load(string a_name)
    {
        if (File.Exists(Application.persistentDataPath + "/" + a_name + ".settings"))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + a_name + ".settings", FileMode.Open);
            Settings loadedSettings = (Settings)bf.Deserialize(file);
            file.Close();

            return loadedSettings;

        }
        else
        {
            return null;
        }
    }
}
