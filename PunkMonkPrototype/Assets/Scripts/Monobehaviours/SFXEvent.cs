﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEvent : MonoBehaviour
{
    [SerializeField] string[] sfx;
    bool inside = false;

    public void PlaySFX(string a_sfx)
    {
        Manager.instance.SfxManager.FindSFXToPlay(a_sfx, gameObject);
    }



}
