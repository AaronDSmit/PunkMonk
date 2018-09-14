using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float toolTipDelay = 1;

    private bool underMouse = false;

    [SerializeField]
    private ActionType type;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            underMouse = true;

            StartCoroutine(DelayShow());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Manager.instance.StateController.CurrentGameState == GameState.battle)
        {
            underMouse = false;

            ToolTip.instance.Hide();
        }
    }

    private IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(toolTipDelay);

        if (underMouse)
        {
            ToolTip.instance.Show(type);
        }
    }
}