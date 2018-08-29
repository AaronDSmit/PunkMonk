using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    private RectTransform rect;

    private GameObject child;

    private EarthUnit earthUnit;

    private LightningUnit lightningUnit;

    [SerializeField]
    private TextMeshProUGUI title;

    [SerializeField]
    private TextMeshProUGUI flavourText;

    [SerializeField]
    private TextMeshProUGUI damage;

    [SerializeField]
    private TextMeshProUGUI range;

    private ActionType buttonType;

    private bool showing = false;

    // Global Manager ref
    public static ToolTip instance;

    public void Init()
    {
        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<EarthUnit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }

        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");

        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<LightningUnit>();
        }
        else
        {
            Debug.LogError("No lightning unit found!");
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            child = transform.GetChild(0).gameObject;

            rect = GetComponent<RectTransform>();

            Hide();
        }
    }



    public void Show(ActionType a_buttonType)
    {
        if (showing)
        {
            return;
        }

        showing = true;
        child.SetActive(true);

        buttonType = a_buttonType;
        ChangeText();

        StartCoroutine(MoveToMouse());
    }

    private void ChangeText()
    {
        if (earthUnit.IsSelected)
        {
            if (buttonType == ActionType.movement)
            {
                title.text = earthUnit.GetAction(0).name;
                flavourText.text = earthUnit.GetAction(0).flavourText;

                range.text = "Range: " + earthUnit.MoveRange.ToString();
                damage.text = "";
            }
            else if (buttonType == ActionType.attack)
            {
                title.text = earthUnit.GetAction(1).name;
                flavourText.text = earthUnit.GetAction(1).flavourText;

                range.text = "Range: " + earthUnit.AttackRange.ToString();
                damage.text = "Damage " + earthUnit.BasicAttackDamage.ToString();
            }
            else if (buttonType == ActionType.specialAttack)
            {
                title.text = earthUnit.GetAction(2).name;
                flavourText.text = earthUnit.GetAction(2).flavourText;

                range.text = "Range: " + earthUnit.AttackRange.ToString();
                damage.text = "Damage (" + earthUnit.MinSpecialDamage.ToString() + " - " + earthUnit.GetSpecialDamage(0).ToString() + ")";
            }
        }
        else
        {
            if (buttonType == ActionType.movement)
            {
                title.text = lightningUnit.GetAction(0).name;
                flavourText.text = lightningUnit.GetAction(0).flavourText;

                range.text = "Range: " + lightningUnit.MoveRange.ToString();
                damage.text = "";
            }
            else if (buttonType == ActionType.attack)
            {
                title.text = lightningUnit.GetAction(1).name;
                flavourText.text = lightningUnit.GetAction(1).flavourText;

                range.text = "Range: " + lightningUnit.AttackRange.ToString();
                damage.text = "Damage " + lightningUnit.BasicAttackDamage.ToString();
            }
            else if (buttonType == ActionType.specialAttack)
            {
                title.text = lightningUnit.GetAction(2).name;
                flavourText.text = lightningUnit.GetAction(2).flavourText;

                range.text = "Range: " + lightningUnit.AttackRange.ToString();
                damage.text = "Damage (" + lightningUnit.MinSpecialDamage.ToString() + " - " + lightningUnit.MaxSpecialDamage.ToString() + ")";
            }
        }
    }

    public void Hide()
    {
        child.SetActive(false);
        showing = false;
    }

    private IEnumerator MoveToMouse()
    {
        while (showing)
        {
            transform.position = Input.mousePosition;

            yield return null;
        }
    }
}