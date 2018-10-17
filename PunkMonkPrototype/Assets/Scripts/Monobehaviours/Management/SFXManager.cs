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



    public void FindSFXToPlay(string a_sfx)
    {
        switch (a_sfx)
        {
            case "Footstep":
                footStepSFX.Post(gameObject);
                break;
            case "EarthBasic":
                basicEarthAttack.Post(gameObject);
                break;
            case "EarthSpecial":
                specialEarthAttack.Post(gameObject);
                break;
            case "LightningBasic":
                basicLightingAttack.Post(gameObject);
                break;
            case "LightningSpecial":
                specialLightingAttack.Post(gameObject);
                break;
            case "MeleeAttack":
                enemyMeleeAttack.Post(gameObject);
                break;
            case "MissileAttack":
                enemyMissileAttack.Post(gameObject);
                break;
            default:
                break;
        }
    }



}
