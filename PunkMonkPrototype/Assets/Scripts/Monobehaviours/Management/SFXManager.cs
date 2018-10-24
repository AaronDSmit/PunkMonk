using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event rainOutside;
    [SerializeField] private AK.Wwise.Event rainInside;
    [SerializeField] private AK.Wwise.Event footStepSFX;
    [SerializeField] private AK.Wwise.Event footStepOutsideSFX;
    [SerializeField] private AK.Wwise.Event basicEarthAttack;
    [SerializeField] private AK.Wwise.Event specialEarthAttack;
    [SerializeField] private AK.Wwise.Event basicLightingAttack;
    [SerializeField] private AK.Wwise.Event specialLightingAttack;
    [SerializeField] private AK.Wwise.Event enemyMeleeAttack;
    [SerializeField] private AK.Wwise.Event enemyMissileAttack;
    [SerializeField] private AK.Wwise.Event stopBattleMusic;

    private bool outside = true;

    private bool isReady = false;

    public bool Ready
    {
        get { return isReady; }
    }

    public void Init()
    {
        rainOutside.Post(gameObject);

        isReady = true;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        if (a_oldstate == GameState.battle)
        {
            stopBattleMusic.Post(gameObject);
        }
    }



    public void FindSFXToPlay(string a_sfx, GameObject a_orginalGO)
    {
        switch (a_sfx)
        {
            case "Footstep":
                if (outside)
                {
                    footStepOutsideSFX.Post(a_orginalGO);
                }
                else
                {
                    footStepSFX.Post(a_orginalGO);
                }
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
            case "EnterInside":
                rainInside.Post(a_orginalGO);
                outside = false;
                break;
            case "EnterOutside":
                rainOutside.Post(a_orginalGO);
                outside = true;
                break;
            default:
                break;
        }
    }



}
