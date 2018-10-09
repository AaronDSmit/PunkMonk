using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    static private SettingsLoader instance;
    static public SettingsLoader Instance { get { return instance; } }

    #region References

    private Settings defaultSettings = null;
    private Settings currentSettings = null;

    #endregion

    #region Properties

    public Settings DefaultSettings
    {
        get { return defaultSettings; }
    }
    public Settings CurrentSettings
    {
        get { return currentSettings; }
    }

    #endregion

    #region Public Methods

    public void Save()
    {
        currentSettings.Save();
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple 'SettingsLoader' scripts found. Destroying duplicates.");
            Destroy(this);
            return;
        }

        defaultSettings = Settings.Load("default");
        if (defaultSettings == null)
        {
            defaultSettings = new Settings();
            defaultSettings.Name = "default";
            defaultSettings.Save();
        }

        currentSettings = Settings.Load("current");
        if (currentSettings == null)
        {
            currentSettings = new Settings();
            currentSettings.Name = "current";
            currentSettings.Save();
        }
    }

    #endregion

}
