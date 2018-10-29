using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{



    private GameSettings settings;

    private bool mute = false;

    private void Awake()
    {
        settings = SettingsLoader.Instance.CurrentSettings;
    }


    private void Start()
    {
        settings.onSettingsChanged += SettingsChanged;
        SettingsChanged();
    }

    private void SettingsChanged()
    {
        mute = settings.MuteAll;

        AkSoundEngine.SetRTPCValue("MasterVolume", mute ? 0 : settings.Master * 100);
        AkSoundEngine.SetRTPCValue("MusicVolume", settings.Music * 100);
        //AkSoundEngine.SetRTPCValue("SFXVolume", settings.SFX);
        //AkSoundEngine.SetRTPCValue("AmbientVolume", settings.Ambient);

    }


    private void OnDestroy()
    {
        // Unsubscribe to the settings changing delegate
        settings.onSettingsChanged -= SettingsChanged;
    }



}
