using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event footStepSFX;
    [SerializeField] private AK.Wwise.Event basicEarthAttack;
    [SerializeField] private AK.Wwise.Event specialEarthAttack;
    [SerializeField] private AK.Wwise.Event basicLightingAttack;
    [SerializeField] private AK.Wwise.Event specialLightingAttack;
    [SerializeField] private AK.Wwise.Event enemyMeleeAttack;
    [SerializeField] private AK.Wwise.Event enemyMissileAttack;



    public void FindSFXToPlay(string a_sfx, GameObject a_orginalGO)
    {
        switch (a_sfx)
        {
            case "Footstep":
                footStepSFX.Post(a_orginalGO);
                break;
            case "EarthBasic":
                basicEarthAttack.Post(a_orginalGO);
                break;
            case "EarthSpecial":
                specialEarthAttack.Post(a_orginalGO);
                break;
            case "LightningBasic":
                basicLightingAttack.Post(a_orginalGO);
                break;
            case "LightningSpecial":
                specialLightingAttack.Post(a_orginalGO);
                break;
            case "MeleeAttack":
                enemyMeleeAttack.Post(a_orginalGO);
                break;
            case "MissileAttack":
                enemyMissileAttack.Post(a_orginalGO);
                break;
            default:
                break;
        }
    }



}
