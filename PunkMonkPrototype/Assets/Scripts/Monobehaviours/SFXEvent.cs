using System.Collections;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Manager.instance.PlayerController.GetComponent<OverworldController>().Controller.gameObject.tag)
        {
            inside = !inside;
            if (inside == true)
            {
                Manager.instance.SfxManager.FindSFXToPlay(sfx[0], gameObject);
            }
            else
            {
                Manager.instance.SfxManager.FindSFXToPlay(sfx[1], gameObject);
            }
        }
    }

}
