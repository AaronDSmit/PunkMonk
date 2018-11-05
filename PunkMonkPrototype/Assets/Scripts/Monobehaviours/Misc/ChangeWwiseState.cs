using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWwiseState : MonoBehaviour {

    [SerializeField] AK.Wwise.State toState;
    [SerializeField] AK.Wwise.State fromState;
    [SerializeField] bool to = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Manager.instance.PlayerController.GetComponent<OverworldController>().Controller.tag)
        {

            to = !to;
            if (to == false)
            {
                AkSoundEngine.SetState((uint)fromState.groupID, (uint)fromState.ID);
                Manager.instance.SfxManager.FindSFXToPlay("EnterOutside", gameObject);
            }
            else
            {
                AkSoundEngine.SetState((uint)toState.groupID, (uint)toState.ID);
                Manager.instance.SfxManager.FindSFXToPlay("EnterInside", gameObject);

            }
        }
    }


}
