using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    static public SettingsLoader Instance { get; private set; }

    #region References

    private GameSettings defaultSettings = null;
    private GameSettings currentSettings = null;

    #endregion

    #region Properties

    public GameSettings DefaultSettings
    {
        get { return defaultSettings; }
    }
    public GameSettings CurrentSettings
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple 'SettingsLoader' scripts found. Destroying duplicates.");
            Destroy(this);
            return;
        }

        defaultSettings = GameSettings.Load("default");
        if (defaultSettings == null)
        {
            defaultSettings = new GameSettings();
            defaultSettings.Name = "default";
            defaultSettings.Save();
        }

        currentSettings = GameSettings.Load("current");
        if (currentSettings == null)
        {
            currentSettings = new GameSettings();
            currentSettings.Name = "current";
            currentSettings.Save();
        }
    }

    #endregion

}
